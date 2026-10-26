import { useEffect, useMemo, useState } from "react";

import toast from "react-hot-toast";

import { useNavigate } from "react-router-dom";

import axios from "axios";

import dayjs from "dayjs";

import "dayjs/locale/pt-br";

import customParseFormat from "dayjs/plugin/customParseFormat";

import { BarChart, XAxis, Bar, Legend, YAxis, Tooltip, ResponsiveContainer, Line } from "recharts";

import { listarTransacoes } from "../Services/TransacaoService";

import { listarComprasParceladas } from "../Services/CompraParceladaService";

import { consultarCartoes, type CartaoResponse } from "../Services/CartaoService";

import type { TransacaoResponse } from "../Types/index";

import type { CompraParceladaResponse } from "../Types/CompraParceladaResponse";

import { CustomTooltip } from "../Components/CustomTooltip";

import { Finalidade } from "../Types/Finalidade";

import nubank from "../assets/Cartoes/Cartao-Nubank.png";
import santander from "../assets/Cartoes/Cartao-Santander.png";
import bradesco from "../assets/Cartoes/Cartao-Bradesco.png";
import inter from "../assets/Cartoes/Cartao-Inter.png";
import amazonPrime from "../assets/Cartoes/Cartao-Amazon-Prime.png";
import bancoBrasil from "../assets/Cartoes/Cartao-Banco-Brasil.png";
import caixa from "../assets/Cartoes/Cartao-Caixa.png";

const CARTOES_DISPONIVEIS = [
  { nome: "Nubank", imagem: "nubank.png", src: nubank },
  { nome: "Santander", imagem: "santander.png", src: santander },
  { nome: "Bradesco", imagem: "bradesco.png", src: bradesco },
  { nome: "Inter", imagem: "inter.png", src: inter },
  { nome: "Amazon Prime", imagem: "amazon-prime.png", src: amazonPrime },
  { nome: "Banco do Brasil", imagem: "banco-do-brasil.png", src: bancoBrasil },
  { nome: "Caixa", imagem: "caixa.png", src: caixa },
];

function obterImagem(imagem: string | null | undefined) {
  if (!imagem) return undefined;

  return CARTOES_DISPONIVEIS.find((cartao) => cartao.imagem === imagem)?.src;
}

dayjs.extend(customParseFormat);

export default function RelatorioFinanceiro() {
  const navigate = useNavigate();

  const [transacoes, setTransacoes] = useState<TransacaoResponse[]>([]);

  const [comprasParceladas, setComprasParceladas] = useState<CompraParceladaResponse[]>([]);

  const [cartoes, setCartoes] = useState<CartaoResponse[]>([]);

  const filtrosSalvos = JSON.parse(localStorage.getItem("filtros-dashboard") || "{}");

  const [mesSelecionado, setMesSelecionado] = useState<string>(filtrosSalvos.mes || "todos");

  const [tipoSelecionado, setTipoSelecionado] = useState<number | "todos">(filtrosSalvos.tipo ?? "todos");

  const [loading, setLoading] = useState(true);

  const [loadingFaturas, setLoadingFaturas] = useState(true);

  const [cartaoSelecionadoId, setCartaoSelecionadoId] = useState<number | null>(null);

  const [mesFaturaSelecionado, setMesFaturaSelecionado] = useState<string>(() => dayjs().format("YYYY-MM"));

  const [animando, setAnimando] = useState(false);

  const [period, setPeriod] = useState<3 | 6 | 12>(6);

  useEffect(() => {
    async function carregarDados() {
      try {
        setLoading(true);

        const [resultadoTransacoes, resultadoCompras, resultadoCartoes] = await Promise.allSettled([
          listarTransacoes(),

          listarComprasParceladas(),

          consultarCartoes(),
        ]);

        if (resultadoTransacoes.status === "fulfilled") {
          setTransacoes(resultadoTransacoes.value);
        } else {
          const error = resultadoTransacoes.reason;

          if (axios.isAxiosError(error)) {
            if (error.response?.status !== 401) toast.error("Erro ao carregar transações");
          } else {
            console.error(error);

            toast.error("Não foi possível carregar as transações");
          }
        }

        if (resultadoCompras.status === "fulfilled") {
          setComprasParceladas(resultadoCompras.value);
        } else {
          const error = resultadoCompras.reason;

          if (axios.isAxiosError(error)) {
            if (error.response?.status !== 401) toast.error("Erro ao carregar o valor da fatura");
          } else {
            console.error(error);

            toast.error("Não foi possível carregar o valor da fatura");
          }
        }

        if (resultadoCartoes.status === "fulfilled") {
          setCartoes(resultadoCartoes.value);
        } else {
          console.error(resultadoCartoes.reason);
        }
      } finally {
        setLoading(false);

        setLoadingFaturas(false);
      }
    }

    carregarDados();
  }, []);

  const transacoesFiltradas = useMemo(() => {
    let resultados = [...transacoes];

    if (mesSelecionado !== "todos") {
      resultados = resultados.filter((t) => {
        const data = dayjs(t.data);

        return data.isValid() && data.format("YYYY-MM") === mesSelecionado;
      });
    }

    if (tipoSelecionado !== "todos") {
      resultados = resultados.filter((t) => t.tipo === Number(tipoSelecionado));
    }

    return resultados;
  }, [mesSelecionado, tipoSelecionado, transacoes]);

  useEffect(() => {
    setAnimando(true);

    const timeout = setTimeout(() => setAnimando(false), 200);

    return () => clearTimeout(timeout);
  }, [mesSelecionado, tipoSelecionado]);

  useEffect(() => {
    localStorage.setItem(
      "filtros-dashboard",

      JSON.stringify({
        mes: mesSelecionado,

        tipo: tipoSelecionado,
      }),
    );
  }, [mesSelecionado, tipoSelecionado]);

  const resumoPorUsuario = useMemo(() => {
    const usuariosMap = new Map<
      number,
      {
        usuarioId: number;

        usuarioNome: string;

        totalReceitas: number;

        totalDespesas: number;

        totalInvestimentos: number;

        saldoFinal: number;
      }
    >();

    transacoesFiltradas.forEach((transacao) => {
      if (!usuariosMap.has(transacao.usuarioId)) {
        usuariosMap.set(transacao.usuarioId, {
          usuarioId: transacao.usuarioId,

          usuarioNome: transacao.usuarioNome,

          totalReceitas: 0,

          totalDespesas: 0,

          totalInvestimentos: 0,

          saldoFinal: 0,
        });
      }

      const usuario = usuariosMap.get(transacao.usuarioId)!;

      const ehInvestimento = transacao.finalidadeCategoria === Finalidade.Investimento;

      if (ehInvestimento) {
        usuario.totalInvestimentos += transacao.valor;
      } else if (transacao.tipo === 1) {
        usuario.totalReceitas += transacao.valor;
      } else if (transacao.tipo === 2) {
        usuario.totalDespesas += transacao.valor;
      }

      usuario.saldoFinal = usuario.totalReceitas - usuario.totalDespesas - usuario.totalInvestimentos;
    });

    return Array.from(usuariosMap.values());
  }, [transacoesFiltradas]);

  const filtrosAtivos = mesSelecionado !== "todos" || tipoSelecionado !== "todos";

  const limparFiltros = () => {
    setMesSelecionado("todos");

    setTipoSelecionado("todos");
  };

  const totalGeralReceitas = useMemo(() => resumoPorUsuario.reduce((acc, curr) => acc + curr.totalReceitas, 0), [resumoPorUsuario]);

  const totalGeralDespesas = useMemo(() => {
    if (tipoSelecionado === 1) return 0;

    let total = 0;

    const filtrarPorMes = (data: dayjs.Dayjs) => {
      if (!data.isValid()) return false;

      if (mesSelecionado === "todos") return true;

      return data.format("YYYY-MM") === mesSelecionado;
    };

    // Transações comuns.
    transacoes.forEach((transacao) => {
      if (transacao.tipo !== 2) return;

      if (transacao.finalidadeCategoria === Finalidade.Investimento) {
        return;
      }

      const transacaoComCartao = transacao as TransacaoResponse & {
        cartaoId?: number | null;
        cartao?: { id: number; nome: string } | null;
      };

      const cartaoId = transacaoComCartao.cartaoId ?? transacaoComCartao.cartao?.id ?? null;

      const dataCompra = dayjs(transacao.data);

      if (!dataCompra.isValid()) return;

      let dataCompetencia = dataCompra;

      // Se for cartão, considera o mês da fatura.
      if (cartaoId != null) {
        const cartao = cartoes.find((c) => c.id === cartaoId);
        const diaFechamento = cartao?.diaFechamento ?? 31;

        if (dataCompra.date() > diaFechamento) {
          dataCompetencia = dataCompra.add(1, "month");
        }
      }

      if (filtrarPorMes(dataCompetencia)) {
        total += Number(transacao.valor) || 0;
      }
    });

    // Compras parceladas.
    comprasParceladas.forEach((compra) => {
      const dados = compra as CompraParceladaResponse & {
        parcelas?: Array<{
          valor: number;
          dataVencimento?: string;
        }>;
      };

      (dados.parcelas ?? []).forEach((parcela) => {
        if (!parcela.dataVencimento) return;

        const dataVencimento = dayjs(parcela.dataVencimento);

        if (filtrarPorMes(dataVencimento)) {
          total += Number(parcela.valor) || 0;
        }
      });
    });

    return total;
  }, [transacoes, comprasParceladas, cartoes, mesSelecionado, tipoSelecionado]);

  const totalGeralInvestimentos = useMemo(() => resumoPorUsuario.reduce((acc, curr) => acc + curr.totalInvestimentos, 0), [resumoPorUsuario]);

  const saldoGeralFinal = totalGeralReceitas - totalGeralDespesas - totalGeralInvestimentos;

  const ultimasTransacoes = useMemo(
    () => [...transacoesFiltradas].sort((a, b) => new Date(b.data).getTime() - new Date(a.data).getTime()).slice(0, 8),

    [transacoesFiltradas],
  );

  const maiorTransacaoId = useMemo(() => {
    if (ultimasTransacoes.length === 0) return null;

    return ultimasTransacoes.reduce((maior, atual) => (Math.abs(atual.valor) > Math.abs(maior.valor) ? atual : maior)).id;
  }, [ultimasTransacoes]);

  function gerarMeses() {
    const meses = [];

    const hoje = new Date();

    for (let i = 0; i < 12; i++) {
      const data = new Date(hoje.getFullYear(), hoje.getMonth() - i, 1);

      meses.push({
        valor: `${data.getFullYear()}-${String(data.getMonth() + 1).padStart(2, "0")}`,

        label: data.toLocaleDateString("pt-BR", { month: "long", year: "numeric" }),
      });
    }

    return meses;
  }

  const dadosPorMes = useMemo(() => {
    const mapa = new Map<string, { receita: number; despesa: number; investimento: number }>();

    const garantirMes = (data: dayjs.Dayjs) => {
      const chave = data.format("MM/YY");

      if (!mapa.has(chave)) {
        mapa.set(chave, {
          receita: 0,
          despesa: 0,
          investimento: 0,
        });
      }

      return mapa.get(chave)!;
    };

    const inicioPeriodo = dayjs()
      .subtract(period - 1, "month")
      .startOf("month");

    const fimPeriodo = dayjs().add(4, "month").endOf("month");

    // Transações comuns
    transacoes.forEach((transacao) => {
      if (tipoSelecionado !== "todos" && transacao.tipo !== Number(tipoSelecionado)) {
        return;
      }

      const dataCompra = dayjs(transacao.data);

      if (!dataCompra.isValid()) return;

      const transacaoComCartao = transacao as TransacaoResponse & {
        cartaoId?: number | null;
        cartao?: { id: number; nome: string } | null;
      };

      const cartaoId = transacaoComCartao.cartaoId ?? transacaoComCartao.cartao?.id ?? null;

      let dataCompetencia = dataCompra;

      // Compras no cartão entram no mês da fatura.
      if (transacao.tipo === 2 && cartaoId != null) {
        const cartao = cartoes.find((c) => c.id === cartaoId);
        const diaFechamento = cartao?.diaFechamento ?? 31;

        if (dataCompra.date() > diaFechamento) {
          dataCompetencia = dataCompra.add(1, "month");
        }
      }

      if (dataCompetencia.isBefore(inicioPeriodo, "month") || dataCompetencia.isAfter(fimPeriodo, "month")) {
        return;
      }

      const item = garantirMes(dataCompetencia);

      if (transacao.finalidadeCategoria === Finalidade.Investimento) {
        item.investimento += transacao.valor;
      } else if (transacao.tipo === 1) {
        item.receita += transacao.valor;
      } else if (transacao.tipo === 2) {
        item.despesa += transacao.valor;
      }
    });

    // Compras parceladas: cada parcela entra no mês do vencimento.
    comprasParceladas.forEach((compra) => {
      const dados = compra as CompraParceladaResponse & {
        parcelas?: Array<{
          valor: number;
          dataVencimento?: string;
        }>;
      };

      (dados.parcelas ?? []).forEach((parcela) => {
        if (!parcela.dataVencimento) return;

        const dataVencimento = dayjs(parcela.dataVencimento);

        if (!dataVencimento.isValid()) return;

        if (dataVencimento.isBefore(inicioPeriodo, "month") || dataVencimento.isAfter(fimPeriodo, "month")) {
          return;
        }

        garantirMes(dataVencimento).despesa += Number(parcela.valor) || 0;
      });
    });

    const dadosOrdenados = Array.from({ length: period + 4 }).map((_, indice) => {
      const data = dayjs()
        .subtract(period - 1, "month")
        .add(indice, "month");

      const chave = data.format("MM/YY");

      const valores = mapa.get(chave) ?? {
        receita: 0,
        despesa: 0,
        investimento: 0,
      };

      return {
        mes: chave,
        receita: valores.receita,
        despesa: valores.despesa,
        investimento: valores.investimento,
      };
    });

    let saldo = 0;

    return dadosOrdenados.map((item) => {
      saldo += item.receita - item.despesa - item.investimento;

      return {
        ...item,
        saldoAcumulado: saldo,
      };
    });
  }, [transacoes, comprasParceladas, cartoes, period, tipoSelecionado]);

  function formatarValor(valor: number) {
    return valor.toLocaleString("pt-BR", { style: "currency", currency: "BRL" });
  }

  const faturasPorCartao = useMemo(() => {
    const mesSelecionado = dayjs(`${mesFaturaSelecionado}-01`);

    const inicioMes = mesSelecionado.startOf("month");

    const fimMes = mesSelecionado.endOf("month");

    const itens: Array<{
      id: number;
      valor: number;
      dataVencimento: string;
      dataCompra: string;
      descricaoCompra: string;
      origem: "parcela" | "avista";
      cartaoId: number | null;
      cartaoNome: string;
    }> = [];

    comprasParceladas.forEach((compra) => {
      const dados = compra as CompraParceladaResponse & {
        cartaoId?: number | null;

        parcelas: Array<{ id: number; valor: number; dataVencimento: string; paga: boolean }>;
      };

      const cartaoId = dados.cartaoId ?? null;

      const cartao = cartoes.find((c) => c.id === cartaoId);

      dados.parcelas

        .filter((parcela) => {
          const vencimento = dayjs(parcela.dataVencimento);

          return !parcela.paga && vencimento.isValid() && !vencimento.isBefore(inicioMes, "day") && !vencimento.isAfter(fimMes, "day");
        })

        .forEach((parcela) =>
          itens.push({
            id: parcela.id,

            valor: parcela.valor,

            dataVencimento: parcela.dataVencimento,

            dataCompra: compra.dataCompra,

            descricaoCompra: compra.descricao,

            origem: "parcela",

            cartaoId,

            cartaoNome: cartao?.nome ?? "Cartão não identificado",
          }),
        );
    });

    (transacoes as (TransacaoResponse & { cartaoId?: number | null; cartao?: { id: number; nome: string } | null })[]).forEach((transacao) => {
      const cartaoId = transacao.cartaoId ?? transacao.cartao?.id ?? null;

      if (transacao.tipo !== 2 || cartaoId == null) return;

      const dataCompra = dayjs(transacao.data);

      if (!dataCompra.isValid()) return;

      const cartao = cartoes.find((c) => c.id === cartaoId);

      const diaFechamento = cartao?.diaFechamento ?? 31;

      const mesFatura = dataCompra.date() > diaFechamento ? dataCompra.add(1, "month") : dataCompra;

      if (mesFatura.isBefore(inicioMes, "month") || mesFatura.isAfter(fimMes, "month")) return;

      itens.push({
        id: transacao.id,

        valor: transacao.valor,

        dataVencimento: mesFatura.format("YYYY-MM-DD"),

        dataCompra: transacao.data,

        descricaoCompra: transacao.descricao,

        origem: "avista",

        cartaoId,

        cartaoNome: cartao?.nome ?? transacao.cartao?.nome ?? "Cartão não identificado",
      });
    });

    return cartoes

      .map((cartao) => {
        const compras = itens.filter((item) => item.cartaoId === cartao.id);

        return {
          cartaoId: cartao.id,

          cartaoNome: cartao.nome,

          total: compras.reduce((soma, item) => soma + item.valor, 0),

          quantidade: compras.length,

          compras,
        };
      })

      .filter((fatura) => fatura.compras.length > 0);
  }, [comprasParceladas, transacoes, cartoes, mesFaturaSelecionado]);

  const historicoFaturas = useMemo(() => {
    const meses = Array.from({ length: period + 4 }, (_, indice) => {
      const dataMes = dayjs()
        .subtract(period - 1, "month")
        .add(indice, "month");

      return {
        chave: dataMes.format("YYYY-MM"),

        mes: dataMes.format("MMM/YY"),

        total: 0,
      };
    });

    const porMes = new Map(meses.map((mes) => [mes.chave, mes]));

    comprasParceladas.forEach((compra) => {
      const dados = compra as CompraParceladaResponse & {
        parcelas?: Array<{ valor: number; dataVencimento?: string; paga?: boolean }>;
      };

      (dados.parcelas ?? []).forEach((parcela) => {
        if (!parcela.dataVencimento) return;

        const vencimento = dayjs(parcela.dataVencimento);

        const mes = vencimento.isValid() ? porMes.get(vencimento.format("YYYY-MM")) : undefined;

        if (mes && !parcela.paga) mes.total += Number(parcela.valor) || 0;
      });
    });

    (
      transacoes as (TransacaoResponse & {
        cartaoId?: number | null;

        cartao?: { id: number; nome: string } | null;
      })[]
    ).forEach((transacao) => {
      const cartaoId = transacao.cartaoId ?? transacao.cartao?.id ?? null;

      if (transacao.tipo !== 2 || cartaoId == null) return;

      const dataCompra = dayjs(transacao.data);

      if (!dataCompra.isValid()) return;

      const cartao = cartoes.find((item) => item.id === cartaoId);

      const diaFechamento = cartao?.diaFechamento ?? 31;

      const dataFatura = dataCompra.date() > diaFechamento ? dataCompra.add(1, "month") : dataCompra;

      const mes = porMes.get(dataFatura.format("YYYY-MM"));

      if (mes) mes.total += Number(transacao.valor) || 0;
    });

    return meses.map(({ mes, total }) => ({ mes, total }));
  }, [period, comprasParceladas, transacoes, cartoes]);

  const totalFaturaAtual = useMemo(() => faturasPorCartao.reduce((total, fatura) => total + fatura.total, 0), [faturasPorCartao]);

  const quantidadeItensFatura = useMemo(() => faturasPorCartao.reduce((total, fatura) => total + fatura.quantidade, 0), [faturasPorCartao]);

  dayjs.locale("pt-br");

  const mesFatura = dayjs(`${mesFaturaSelecionado}-01`).format("MMMM [de] YYYY");

  // Se nenhum cartão foi escolhido, mostra o primeiro por padrão.
  const cartaoAtivoId = cartaoSelecionadoId ?? cartoes[0]?.id ?? null;

  return (
    <main className="min-h-full w-full overflow-x-hidden bg-[#3E5C76] px-3 py-4 text-[#0D1321] sm:px-4 lg:px-6">
      <div className="mx-auto w-full max-w-400 space-y-4">
        {/* Cabeçalho, filtros e indicadores */}
        <section className="rounded-2xl border border-[#748CAB]/40 bg-white p-4 shadow-sm">
          <div className="flex flex-col gap-3 lg:flex-row lg:items-end lg:justify-between">
            <div>
              <p className="text-[11px] font-bold uppercase tracking-[0.16em] text-[#3E5C76]">Sua vida financeira, em perspectiva</p>
              <h1 className="text-2xl font-bold tracking-tight text-[#1D2D44]">Resumo financeiro</h1>
            </div>

            <div className="flex flex-wrap items-end gap-2">
              <label className="flex flex-col gap-1 text-[11px] font-semibold text-[#3E5C76]">
                Período
                <select
                  id="select-mes"
                  value={mesSelecionado}
                  onChange={(e) => setMesSelecionado(e.target.value)}
                  className="min-w-40 rounded-lg border border-[#748CAB]/30 bg-[#F0EBD8] px-3 py-2 text-sm font-medium text-[#1D2D44] outline-none transition focus:border-[#3E5C76] focus:ring-4 focus:ring-[#3E5C76]/20"
                >
                  <option value="todos">Todos os meses</option>
                  {gerarMeses().map((mes) => (
                    <option key={mes.valor} value={mes.valor}>
                      {mes.label.charAt(0).toUpperCase() + mes.label.slice(1)}
                    </option>
                  ))}
                </select>
              </label>
              <label className="flex flex-col gap-1 text-[11px] font-semibold text-[#3E5C76]">
                Tipo de lançamento
                <select
                  id="select-tipo"
                  value={tipoSelecionado}
                  onChange={(e) => setTipoSelecionado(e.target.value === "todos" ? "todos" : Number(e.target.value))}
                  className="min-w-36 rounded-lg border border-[#748CAB]/30 bg-[#F0EBD8] px-3 py-2 text-sm font-medium text-[#1D2D44] outline-none transition focus:border-[#3E5C76] focus:ring-4 focus:ring-[#3E5C76]/20"
                >
                  <option value="todos">Todos os tipos</option>
                  <option value={1}>Receitas</option>
                  <option value={2}>Despesas</option>
                </select>
              </label>
              <button
                onClick={limparFiltros}
                disabled={!filtrosAtivos}
                className="rounded-lg border border-[#748CAB]/40 px-3 py-2 text-sm font-bold text-[#1D2D44] transition hover:bg-[#F0EBD8] disabled:cursor-not-allowed disabled:opacity-40"
              >
                Limpar
              </button>
            </div>
          </div>

          {/* Indicadores */}
          <div className="mt-4 grid grid-cols-2 gap-2.5 border-t border-[#748CAB]/30 pt-4 lg:grid-cols-4">
            <article className="flex items-center gap-3 rounded-xl border border-[#86BA90]/60 bg-white p-3 transition hover:shadow-sm">
              <span className="flex h-9 w-9 shrink-0 items-center justify-center rounded-lg bg-[#86BA90]/25 text-lg font-bold text-[#3E7F54]">↙</span>
              <div className="min-w-0">
                <p className="text-xs font-medium text-[#3E5C76]">Receitas</p>
                <p className="wrap-break-word text-lg font-bold tracking-tight text-[#3E7F54] sm:text-xl">{formatarValor(totalGeralReceitas)}</p>
              </div>
            </article>
            <article className="flex items-center gap-3 rounded-xl border border-[#C44536]/30 bg-white p-3 transition hover:shadow-sm">
              <span className="flex h-9 w-9 shrink-0 items-center justify-center rounded-lg bg-[#C44536]/10 text-lg font-bold text-[#C44536]">↗</span>
              <div className="min-w-0">
                <p className="text-xs font-medium text-[#3E5C76]">Despesas</p>
                <p className="wrap-break-word text-lg font-bold tracking-tight text-[#C44536] sm:text-xl">{formatarValor(totalGeralDespesas)}</p>
              </div>
            </article>
            <article className="flex items-center gap-3 rounded-xl border border-[#748CAB]/40 bg-white p-3 transition hover:shadow-sm">
              <span className="flex h-9 w-9 shrink-0 items-center justify-center rounded-lg bg-[#748CAB]/20 text-lg font-bold text-[#3E5C76]">◇</span>
              <div className="min-w-0">
                <p className="text-xs font-medium text-[#3E5C76]">Investimentos</p>
                <p className="wrap-break-word text-lg font-bold tracking-tight text-[#3E5C76] sm:text-xl">{formatarValor(totalGeralInvestimentos)}</p>
              </div>
            </article>
            <article className="flex items-center gap-3 rounded-xl border border-[#0D1321] bg-[#0D1321] p-3 transition hover:shadow-sm">
              <span className="flex h-9 w-9 shrink-0 items-center justify-center rounded-lg bg-white/10 text-lg font-bold text-white">=</span>
              <div className="min-w-0">
                <p className="text-xs font-medium text-white/75">Saldo final</p>
                <p
                  className={`wrap-break-word text-lg font-bold tracking-tight sm:text-xl ${saldoGeralFinal >= 0 ? "text-[#86BA90]" : "text-[#C44536]"}`}
                >
                  {formatarValor(saldoGeralFinal)}
                </p>
              </div>
            </article>
          </div>
        </section>

        {/* Conteúdo principal em duas colunas */}
        <div className="grid grid-cols-1 items-stretch gap-4 xl:grid-cols-[minmax(0,3fr)_minmax(0,2fr)]">
          {/* Coluna esquerda: evolução + últimas transações */}
          <div className="flex min-w-0 flex-col gap-4">
            <section className="rounded-2xl border border-[#748CAB]/30 bg-white p-4 shadow-sm">
              <div className="mb-3 flex flex-wrap items-center justify-between gap-2">
                <div>
                  <p className="text-[11px] font-bold uppercase tracking-[0.16em] text-[#3E5C76]">Visão de longo prazo</p>
                  <h2 className="text-lg font-bold text-[#1D2D44]">Evolução financeira</h2>
                </div>
                <div className="inline-flex w-fit rounded-lg bg-[#F0EBD8] p-0.5" aria-label="Período do gráfico">
                  {[3, 6, 12].map((p) => (
                    <button
                      key={p}
                      onClick={() => setPeriod(p as 3 | 6 | 12)}
                      aria-pressed={period === p}
                      className={`rounded-md px-2.5 py-1.5 text-xs font-bold transition ${period === p ? "bg-white text-[#1D2D44] shadow-sm" : "text-[#3E5C76] hover:text-[#0D1321]"}`}
                    >
                      {p} meses
                    </button>
                  ))}
                </div>
              </div>
              <div className="h-64 w-full">
                <ResponsiveContainer width="100%" height="100%">
                  <BarChart data={dadosPorMes} barGap={6} margin={{ top: 8, right: 10, left: 0, bottom: 0 }}>
                    <XAxis dataKey="mes" tickLine={false} axisLine={false} tick={{ fill: "#3E5C76", fontSize: 12 }} />
                    <YAxis
                      tickFormatter={(valor) => `R$ ${valor}`}
                      width={75}
                      tickLine={false}
                      axisLine={false}
                      tick={{ fill: "#3E5C76", fontSize: 11 }}
                    />
                    <Tooltip content={<CustomTooltip />} />
                    <Legend wrapperStyle={{ fontSize: 12, paddingTop: 6 }} />
                    <Bar dataKey="receita" name="Receitas" fill="#86BA90" radius={[5, 5, 0, 0]} />
                    <Bar dataKey="despesa" name="Despesas" fill="#C44536" radius={[5, 5, 0, 0]} />
                    <Bar dataKey="investimento" name="Investimentos" fill="#748CAB" radius={[5, 5, 0, 0]} />
                    <Line
                      type="monotone"
                      dataKey="saldoAcumulado"
                      name="Saldo acumulado"
                      stroke="#1D2D44"
                      strokeWidth={3}
                      dot={{ r: 3, fill: "#1D2D44" }}
                      activeDot={{ r: 5 }}
                    />
                  </BarChart>
                </ResponsiveContainer>
              </div>
            </section>

            {/* Transações recentes */}
            <section className="flex-1 overflow-hidden rounded-2xl border border-[#748CAB]/40 bg-white shadow-sm">
              <div className="flex items-center justify-between gap-3 border-b border-[#748CAB]/40 px-4 py-3">
                <div>
                  <div className="flex items-center gap-2">
                    <span className="h-2 w-2 rounded-full bg-[#86BA90]" />
                    <p className="text-[11px] font-bold uppercase tracking-[0.16em] text-[#3E5C76]">Atividade recente</p>
                  </div>
                  <h2 className="text-lg font-bold tracking-tight text-[#1D2D44]">Últimas transações</h2>
                </div>

                <button
                  onClick={() => navigate("/transacao")}
                  className="inline-flex shrink-0 items-center justify-center gap-1.5 rounded-lg border border-[#748CAB]/40 bg-[#F0EBD8]/50 px-3 py-2 text-sm font-bold text-[#1D2D44] transition duration-200 hover:border-[#3E5C76] hover:bg-[#F0EBD8] focus:outline-none focus:ring-4 focus:ring-[#3E5C76]/20"
                >
                  Ver todas
                  <span>→</span>
                </button>
              </div>

              <div className="p-3 sm:p-4">
                {loading ? (
                  <div className="space-y-2">
                    {[...Array(4)].map((_, i) => (
                      <div key={i} className="flex animate-pulse items-center gap-3 rounded-lg border border-[#748CAB]/20 p-3">
                        <div className="h-9 w-9 shrink-0 rounded-lg bg-[#748CAB]/30" />

                        <div className="flex-1 space-y-2">
                          <div className="h-3.5 w-1/2 rounded bg-[#748CAB]/30" />
                          <div className="h-3 w-1/4 rounded bg-[#F0EBD8]" />
                        </div>

                        <div className="h-4 w-20 rounded bg-[#748CAB]/30" />
                      </div>
                    ))}
                  </div>
                ) : ultimasTransacoes.length === 0 ? (
                  <div className="rounded-lg border border-dashed border-[#748CAB]/40 bg-[#F0EBD8]/30 px-4 py-6 text-center">
                    <p className="font-semibold text-[#1D2D44]">Nenhuma transação encontrada</p>

                    <p className="mt-1 text-sm text-[#3E5C76]">Não encontramos lançamentos para os filtros selecionados.</p>

                    {filtrosAtivos && (
                      <button
                        onClick={limparFiltros}
                        className="mt-3 rounded-lg px-3 py-1.5 text-sm font-bold text-[#1D2D44] transition hover:bg-[#F0EBD8]"
                      >
                        Limpar filtros
                      </button>
                    )}
                  </div>
                ) : (
                  <div className={`space-y-2 transition-all duration-200 ${animando ? "translate-y-1 opacity-60" : "opacity-100"}`}>
                    {ultimasTransacoes.map((t) => {
                      const isEntrada = t.tipo === 1;
                      const isMaiorTransacao = t.id === maiorTransacaoId;

                      return (
                        <article
                          key={t.id}
                          className={`flex items-center justify-between gap-3 rounded-lg border px-3 py-2.5 transition duration-200 hover:shadow-sm ${
                            isMaiorTransacao
                              ? "border-[#748CAB] bg-[#F0EBD8]/40 hover:border-[#3E5C76]"
                              : "border-[#748CAB]/20 bg-white hover:border-[#748CAB]"
                          }`}
                        >
                          <div className="flex min-w-0 items-center gap-3">
                            <div
                              className={`flex h-9 w-9 shrink-0 items-center justify-center rounded-lg text-lg font-bold ${
                                isEntrada ? "bg-[#86BA90]/25 text-[#3E7F54]" : "bg-[#C44536]/10 text-[#C44536]"
                              }`}
                              aria-label={isEntrada ? "Entrada" : "Saída"}
                            >
                              {isEntrada ? "↙" : "↗"}
                            </div>

                            <div className="min-w-0">
                              <p className="truncate text-sm font-semibold text-[#1D2D44]">{t.descricao}</p>

                              <div className="mt-0.5 flex flex-wrap items-center gap-x-2 gap-y-0.5 text-xs text-[#3E5C76]">
                                <span>{dayjs(t.data).isValid() ? dayjs(t.data).format("DD/MM/YYYY") : t.data}</span>

                                <span className="h-1 w-1 rounded-full bg-[#748CAB]" />

                                <span className={`font-medium ${isEntrada ? "text-[#3E7F54]" : "text-[#C44536]"}`}>
                                  {isEntrada ? "Entrada" : "Saída"}
                                </span>

                                {isMaiorTransacao && (
                                  <span className="rounded bg-[#748CAB]/25 px-1.5 py-0.5 text-[10px] font-bold uppercase tracking-wide text-[#1D2D44]">
                                    Maior valor
                                  </span>
                                )}
                              </div>
                            </div>
                          </div>

                          <p className={`shrink-0 text-base font-bold tracking-tight ${isEntrada ? "text-[#3E7F54]" : "text-[#C44536]"}`}>
                            {isEntrada ? "+" : "−"} {formatarValor(t.valor)}
                          </p>
                        </article>
                      );
                    })}
                  </div>
                )}
              </div>
            </section>
          </div>

          {/* Coluna direita: fatura e cartões */}
          <article className="flex min-w-0 flex-col overflow-hidden rounded-2xl border border-[#748CAB]/40 bg-white shadow-sm">
            {/* Cabeçalho */}
            <div className="bg-linear-to-br from-[#0D1321] to-[#3E5C76] px-4 py-3 text-white">
              <div className="flex flex-wrap items-center justify-between gap-3">
                <div>
                  <p className="text-[11px] font-bold uppercase tracking-[0.16em] text-[#F0EBD8]">Cartões de crédito</p>
                  <h2 className="text-lg font-bold leading-tight">Fatura do mês</h2>
                  <p className="text-xs capitalize text-white/70">{mesFatura}</p>
                </div>

                <div className="flex items-end gap-2">
                  <label className="flex flex-col gap-1 text-[11px] font-medium text-white/80">
                    Mês da fatura
                    <input
                      type="month"
                      value={mesFaturaSelecionado}
                      onChange={(event) => setMesFaturaSelecionado(event.target.value)}
                      className="rounded-lg border border-white/30 bg-white px-2.5 py-1.5 text-sm text-[#1D2D44] outline-none transition focus:ring-4 focus:ring-white/20"
                    />
                  </label>
                  <button
                    onClick={() => navigate("/transacao")}
                    className="inline-flex items-center justify-center rounded-lg bg-white px-3 py-2 text-xs font-bold text-[#1D2D44] shadow-sm transition hover:bg-[#F0EBD8] focus:outline-none focus:ring-4 focus:ring-white/20"
                  >
                    Parceladas
                    <span className="ml-1.5">→</span>
                  </button>
                </div>
              </div>
            </div>

            <div className="flex flex-1 flex-col gap-4 p-4">
              {/* Total de todos os cartões */}
              <div className="flex items-center justify-between gap-3 rounded-xl bg-[#F0EBD8] px-4 py-3">
                <div className="min-w-0">
                  <p className="text-xs font-medium text-[#1D2D44]">Total gasto em todos os cartões neste mês</p>

                  <p className="wrap-break-word text-2xl font-bold tracking-tight text-[#C44536]">
                    {loadingFaturas ? "Carregando..." : formatarValor(totalFaturaAtual)}
                  </p>
                </div>

                <p className="shrink-0 text-right text-xs text-[#1D2D44]/70">
                  {loadingFaturas
                    ? "Buscando faturas..."
                    : `${quantidadeItensFatura} ${quantidadeItensFatura === 1 ? "lançamento" : "lançamentos"} neste mês`}
                </p>
              </div>

              {/* Planejamento */}
              <div>
                <div className="mb-2 flex items-baseline justify-between gap-2">
                  <h3 className="text-sm font-bold text-[#1D2D44]">Fatura total por mês</h3>
                  <p className="text-[11px] font-bold uppercase tracking-[0.14em] text-[#3E5C76]">Planejamento</p>
                </div>
                <div className="h-36 w-full">
                  <ResponsiveContainer width="100%" height="100%">
                    <BarChart data={historicoFaturas} margin={{ top: 4, right: 8, left: 0, bottom: 0 }}>
                      <XAxis dataKey="mes" tickLine={false} axisLine={false} tick={{ fill: "#3E5C76", fontSize: 11 }} />
                      <YAxis
                        tickFormatter={(valor) => `R$${valor}`}
                        width={62}
                        tickLine={false}
                        axisLine={false}
                        tick={{ fill: "#3E5C76", fontSize: 11 }}
                      />
                      <Tooltip formatter={(valor) => formatarValor(Number(valor))} contentStyle={{ borderRadius: 12, borderColor: "#748CAB" }} />
                      <Bar dataKey="total" name="Fatura total" fill="#C44536" radius={[6, 6, 0, 0]} maxBarSize={36} />
                    </BarChart>
                  </ResponsiveContainer>
                </div>
              </div>

              {/* Seletor de cartões */}
              <div className="border-t border-[#748CAB]/30 pt-4">
                <div className="mb-2">
                  <h3 className="text-sm font-bold text-[#1D2D44]">Seus cartões</h3>
                  <p className="text-xs text-[#3E5C76]">Selecione um cartão para consultar os lançamentos da fatura.</p>
                </div>

                {cartoes.length > 0 ? (
                  <div className="grid grid-cols-1 gap-2 sm:grid-cols-2">
                    {cartoes.map((cartao) => {
                      const selecionado = cartaoAtivoId === cartao.id;

                      const faturaCartao = faturasPorCartao.find((fatura) => fatura.cartaoId === cartao.id);

                      return (
                        <button
                          key={cartao.id}
                          type="button"
                          onClick={() => setCartaoSelecionadoId(cartao.id)}
                          className={`flex items-center gap-2.5 rounded-lg border p-2 text-left transition ${
                            selecionado
                              ? "border-[#86BA90] bg-[#86BA90]/15 ring-2 ring-[#86BA90]/40"
                              : "border-[#748CAB]/40 bg-white hover:border-[#86BA90] hover:bg-[#86BA90]/10"
                          }`}
                        >
                          {obterImagem(cartao.imagem) ? (
                            <img
                              src={obterImagem(cartao.imagem)}
                              alt={`Cartão ${cartao.nome}`}
                              className="h-9 w-12 shrink-0 rounded-md object-contain"
                            />
                          ) : (
                            <div className="flex h-9 w-12 shrink-0 items-center justify-center rounded-md bg-linear-to-br from-[#0D1321] to-[#3E5C76] text-white">
                              <svg
                                xmlns="http://www.w3.org/2000/svg"
                                viewBox="0 0 24 24"
                                fill="none"
                                stroke="currentColor"
                                strokeWidth="1.5"
                                className="h-5 w-5"
                              >
                                <rect x="2" y="5" width="20" height="14" rx="2" />
                                <path d="M2 10h20" />
                              </svg>
                            </div>
                          )}

                          <div className="min-w-0 flex-1">
                            <p className="truncate text-sm font-bold text-[#1D2D44]">{cartao.nome}</p>

                            <p className="text-xs text-[#3E5C76]">{formatarValor(faturaCartao?.total ?? 0)}</p>
                          </div>

                          <div
                            className={`flex h-4 w-4 shrink-0 items-center justify-center rounded-full border ${
                              selecionado ? "border-[#86BA90] bg-[#86BA90] text-[#0D1321]" : "border-[#748CAB]/40"
                            }`}
                          >
                            {selecionado && (
                              <svg
                                xmlns="http://www.w3.org/2000/svg"
                                viewBox="0 0 24 24"
                                fill="none"
                                stroke="currentColor"
                                strokeWidth="3"
                                className="h-2.5 w-2.5"
                              >
                                <path d="m5 12 4 4L19 6" />
                              </svg>
                            )}
                          </div>
                        </button>
                      );
                    })}
                  </div>
                ) : (
                  <p className="rounded-lg border border-dashed border-[#748CAB]/40 p-4 text-center text-sm text-[#3E5C76]">
                    Nenhum cartão cadastrado.
                  </p>
                )}
              </div>

              {/* Lançamentos do cartão selecionado */}
              {cartaoAtivoId !== null && (
                <div className="flex min-h-0 flex-1 flex-col border-t border-[#748CAB]/40 pt-4">
                  {(() => {
                    const cartao = cartoes.find((item) => item.id === cartaoAtivoId);

                    const fatura = faturasPorCartao.find((item) => item.cartaoId === cartaoAtivoId);

                    if (!cartao) return null;

                    return (
                      <>
                        {/* Cabeçalho da fatura */}
                        <div className="mb-3 flex items-center justify-between gap-3 rounded-xl border border-[#748CAB]/40 bg-[#F0EBD8]/50 px-3 py-2.5">
                          <div className="min-w-0">
                            <p className="text-[11px] font-bold uppercase tracking-[0.14em] text-[#3E5C76]">Extrato do cartão</p>

                            <h3 className="truncate text-base font-bold text-[#1D2D44]">{cartao.nome}</h3>

                            <p className="text-xs text-[#1D2D44]/65">
                              {loadingFaturas
                                ? "Atualizando lançamentos..."
                                : `${fatura?.compras.length ?? 0} ${fatura?.compras.length === 1 ? "lançamento" : "lançamentos"} nesta fatura`}
                            </p>
                          </div>

                          <div className="shrink-0 text-right">
                            <p className="text-[11px] font-medium text-[#1D2D44]/65">Total da fatura</p>

                            <p className="text-xl font-bold tracking-tight text-[#1D2D44]">{formatarValor(fatura?.total ?? 0)}</p>
                          </div>
                        </div>

                        {/* Lista de compras */}
                        {loadingFaturas ? (
                          <div className="space-y-2">
                            {[...Array(3)].map((_, index) => (
                              <div key={index} className="flex animate-pulse items-center gap-3 rounded-lg border border-[#748CAB]/20 p-3">
                                <div className="h-8 w-8 shrink-0 rounded-lg bg-[#748CAB]/30" />

                                <div className="flex-1 space-y-2">
                                  <div className="h-3.5 w-2/3 rounded bg-[#748CAB]/30" />
                                  <div className="h-3 w-1/3 rounded bg-[#F0EBD8]" />
                                </div>

                                <div className="h-4 w-16 rounded bg-[#748CAB]/30" />
                              </div>
                            ))}
                          </div>
                        ) : fatura && fatura.compras.length > 0 ? (
                          <div className="flex min-h-0 flex-1 flex-col">
                            <div className="mb-2 flex items-center justify-between px-1">
                              <p className="text-sm font-semibold text-[#1D2D44]">Compras da fatura</p>

                              <p className="text-xs text-[#748CAB]">
                                {fatura.compras.length} {fatura.compras.length === 1 ? "compra" : "compras"}
                              </p>
                            </div>

                            <div className="relative min-h-60 flex-1">
                              <div className="absolute inset-0 space-y-1.5 overflow-y-auto pr-1">
                                {fatura.compras.map((compra) => {
                                  const isParcela = compra.origem === "parcela";

                                  return (
                                    <div
                                      key={`${compra.origem}-${compra.id}`}
                                      className="flex items-center justify-between gap-3 rounded-lg border border-[#748CAB]/30 bg-white px-3 py-2 transition duration-200 hover:border-[#3E5C76] hover:bg-[#F0EBD8]"
                                    >
                                      <div className="flex min-w-0 items-center gap-2.5">
                                        <div className={`h-8 w-1 shrink-0 rounded-full ${isParcela ? "bg-[#C44536]" : "bg-[#86BA90]"}`} />

                                        <div className="min-w-0">
                                          <p className="truncate text-sm font-semibold text-[#1D2D44]">{compra.descricaoCompra}</p>

                                          <div className="mt-0.5 flex flex-wrap items-center gap-x-2 gap-y-0.5">
                                            <span
                                              className={`rounded px-1.5 py-0.5 text-[10px] font-bold uppercase tracking-wide ${
                                                isParcela ? "bg-[#C44536]/10 text-[#C44536]" : "bg-[#86BA90]/25 text-[#3E7F54]"
                                              }`}
                                            >
                                              {isParcela ? "Parcela" : "Compra à vista"}
                                            </span>

                                            <span className="text-xs text-[#3E5C76]">
                                              Data da compra: {dayjs(compra.dataCompra).format("DD/MM/YYYY")}
                                            </span>
                                          </div>
                                        </div>
                                      </div>

                                      <p className="shrink-0 text-sm font-bold text-[#1D2D44]">{formatarValor(compra.valor)}</p>
                                    </div>
                                  );
                                })}
                              </div>
                            </div>
                          </div>
                        ) : (
                          <div className="rounded-lg border border-dashed border-[#748CAB]/40 bg-[#F0EBD8] px-4 py-6 text-center">
                            <p className="font-semibold text-[#1D2D44]">Nenhuma compra encontrada</p>

                            <p className="mt-1 text-sm text-[#3E5C76]">Não há compras pendentes neste cartão para {mesFatura}.</p>
                          </div>
                        )}
                      </>
                    );
                  })()}
                </div>
              )}

              {!loadingFaturas && faturasPorCartao.length === 0 && (
                <p className="rounded-lg border border-dashed border-[#748CAB]/40 px-4 py-3 text-center text-sm text-[#3E5C76]">
                  Nenhuma compra pendente nas faturas de {mesFatura}.
                </p>
              )}
            </div>
          </article>
        </div>

        {!loading && resumoPorUsuario.length === 0 && (
          <div className="rounded-xl border border-dashed border-[#748CAB]/50 bg-white p-4 text-center text-sm text-[#3E5C76]">
            Não há lançamentos para os filtros selecionados.
          </div>
        )}
      </div>
    </main>
  );
}
