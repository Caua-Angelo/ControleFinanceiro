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
import bancoBrasil from "../assets/Cartoes/Cartao-Canco-Brasil.png";
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
    () => [...transacoesFiltradas].sort((a, b) => new Date(b.data).getTime() - new Date(a.data).getTime()).slice(0, 5),

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
    const hoje = dayjs();

    const inicioMes = hoje.startOf("month");

    const fimMes = hoje.endOf("month");

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
  }, [comprasParceladas, transacoes, cartoes]);

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

  const mesFatura = dayjs().format("MMMM [de] YYYY");

  return (
    <main className="min-h-full w-full overflow-x-hidden bg-[#E8DCC6] px-3 py-5 text-slate-800 sm:px-5 lg:px-7">
      <div className="mx-auto w-full max-w-400 space-y-5">
        {/* Cabeçalho e filtros */}
        <section className="rounded-3xl border border-[#D9C7AB] bg-white p-5 shadow-sm sm:p-7">
          <div className="flex flex-col gap-5 xl:flex-row xl:items-end xl:justify-between">
            <div>
              <p className="mb-2 text-xs font-bold uppercase tracking-[0.18em] text-[#87A878]">Sua vida financeira, em perspectiva</p>
              <h1 className="text-3xl font-bold tracking-tight text-[#5B3A29] sm:text-4xl">Resumo financeiro</h1>
              <p className="mt-2 max-w-2xl text-sm leading-6 text-slate-500">
                Acompanhe entradas, despesas, investimentos e compromissos do cartão em um só lugar.
              </p>
            </div>
            <div className="flex flex-col gap-3 sm:items-end">
              <button
                onClick={limparFiltros}
                disabled={!filtrosAtivos}
                className="self-end rounded-lg border border-[#D9C7AB] px-3 py-2 text-xs font-bold text-[#5B3A29] transition hover:bg-[#F4EBDD] disabled:cursor-not-allowed disabled:opacity-40"
              >
                Limpar filtros
              </button>
              <div className="flex flex-col gap-3 sm:flex-row sm:items-end">
                <label className="flex flex-col gap-1.5 text-xs font-semibold text-slate-500">
                  Período
                  <select
                    id="select-mes"
                    value={mesSelecionado}
                    onChange={(e) => setMesSelecionado(e.target.value)}
                    className="min-w-44 rounded-xl border border-slate-200 bg-[#FFF9F0] px-3 py-2.5 text-sm font-medium text-slate-700 outline-none transition focus:border-[#87A878] focus:ring-4 focus:ring-[#87A878]/20"
                  >
                    <option value="todos">Todos os meses</option>
                    {gerarMeses().map((mes) => (
                      <option key={mes.valor} value={mes.valor}>
                        {mes.label.charAt(0).toUpperCase() + mes.label.slice(1)}
                      </option>
                    ))}
                  </select>
                </label>
                <label className="flex flex-col gap-1.5 text-xs font-semibold text-slate-500">
                  Tipo de lançamento
                  <select
                    id="select-tipo"
                    value={tipoSelecionado}
                    onChange={(e) => setTipoSelecionado(e.target.value === "todos" ? "todos" : Number(e.target.value))}
                    className="min-w-40 rounded-xl border border-slate-200 bg-[#FFF9F0] px-3 py-2.5 text-sm font-medium text-slate-700 outline-none transition focus:border-[#87A878] focus:ring-4 focus:ring-[#87A878]/20"
                  >
                    <option value="todos">Todos os tipos</option>
                    <option value={1}>Receitas</option>
                    <option value={2}>Despesas</option>
                  </select>
                </label>
              </div>
            </div>
          </div>

          {/* Indicadores integrados ao resumo financeiro */}
          <div className="mt-5 grid grid-cols-1 gap-3 border-t border-[#E8DCC6] pt-5 sm:grid-cols-2 2xl:grid-cols-4">
            <article className="relative overflow-hidden rounded-2xl border border-[#D5E2CC] bg-white p-5 shadow-sm transition hover:-translate-y-0.5 hover:shadow-md">
              <div className="absolute right-0 top-0 h-20 w-20 rounded-bl-full bg-[#EEF2E8]" />
              <div className="relative flex items-center gap-3">
                <span className="flex h-11 w-11 items-center justify-center rounded-xl bg-[#EEF2E8] text-xl font-bold text-[#587B4C]">↙</span>
                <div>
                  <p className="text-sm font-medium text-slate-500">Receitas</p>
                  <p className="mt-1 text-xs text-[#587B4C]">Entradas no período</p>
                </div>
              </div>
              <p className="relative mt-5 wrap-break-word text-2xl font-bold tracking-tight text-[#587B4C] sm:text-3xl">
                {formatarValor(totalGeralReceitas)}
              </p>
            </article>
            <article className="relative overflow-hidden rounded-2xl border border-[#E8C7B4] bg-white p-5 shadow-sm transition hover:-translate-y-0.5 hover:shadow-md">
              <div className="absolute right-0 top-0 h-20 w-20 rounded-bl-full bg-[#F8E9DF]" />
              <div className="relative flex items-center gap-3">
                <span className="flex h-11 w-11 items-center justify-center rounded-xl bg-[#F8E9DF] text-xl font-bold text-[#A94F2B]">↗</span>
                <div>
                  <p className="text-sm font-medium text-slate-500">Despesas</p>
                  <p className="mt-1 text-xs text-[#A94F2B]">Saídas no período</p>
                </div>
              </div>
              <p className="relative mt-5 wrap-break-word text-2xl font-bold tracking-tight text-[#A94F2B] sm:text-3xl">
                {formatarValor(totalGeralDespesas)}
              </p>
            </article>
            <article className="relative overflow-hidden rounded-2xl border border-[#E5D6B5] bg-white p-5 shadow-sm transition hover:-translate-y-0.5 hover:shadow-md">
              <div className="absolute right-0 top-0 h-20 w-20 rounded-bl-full bg-[#F6EEDB]" />
              <div className="relative flex items-center gap-3">
                <span className="flex h-11 w-11 items-center justify-center rounded-xl bg-[#F6EEDB] text-xl font-bold text-[#94732F]">◇</span>
                <div>
                  <p className="text-sm font-medium text-slate-500">Investimentos</p>
                  <p className="mt-1 text-xs text-[#94732F]">Valores investidos</p>
                </div>
              </div>
              <p className="relative mt-5 wrap-break-word text-2xl font-bold tracking-tight text-[#94732F] sm:text-3xl">
                {formatarValor(totalGeralInvestimentos)}
              </p>
            </article>
            <article className="relative overflow-hidden rounded-2xl border border-[#D9C7AB] bg-[#5B3A29] p-5 shadow-sm transition hover:-translate-y-0.5 hover:shadow-md">
              <div className="absolute -right-4 -top-8 h-28 w-28 rounded-full border-18 border-white/5" />
              <div className="relative flex items-center gap-3">
                <span className="flex h-11 w-11 items-center justify-center rounded-xl bg-white/10 text-xl font-bold text-white">=</span>
                <div>
                  <p className="text-sm font-medium text-white/75">Saldo final</p>
                  <p className="mt-1 text-xs text-white/60">Receitas menos saídas</p>
                </div>
              </div>
              <p
                className={`relative mt-5 wrap-break-word text-2xl font-bold tracking-tight sm:text-3xl ${saldoGeralFinal >= 0 ? "text-white" : "text-[#F0C8AE]"}`}
              >
                {formatarValor(saldoGeralFinal)}
              </p>
            </article>
          </div>
        </section>
        <div className="flex gap-5 flex-col 2xl:flex-row">
          {/* Evolução financeira */}
          <section className=" w-full rounded-2xl border border-slate-200 bg-white p-5 shadow-sm sm:p-6">
            <div className="mb-5 flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
              <div>
                <p className="text-xs font-bold uppercase tracking-[0.16em] text-[#87A878]">Visão de longo prazo</p>
                <h2 className="mt-1 text-xl font-bold text-[#5B3A29] sm:text-2xl">Evolução financeira</h2>
                <p className="mt-1 text-sm text-slate-500">Compare receitas, despesas, investimentos e saldo acumulado.</p>
              </div>
              <div className="inline-flex w-fit rounded-xl bg-slate-100 p-1" aria-label="Período do gráfico">
                {[3, 6, 12].map((p) => (
                  <button
                    key={p}
                    onClick={() => setPeriod(p as 3 | 6 | 12)}
                    aria-pressed={period === p}
                    className={`rounded-lg px-3 py-2 text-sm font-bold transition ${period === p ? "bg-white text-[#5B3A29] shadow-sm" : "text-slate-500 hover:text-slate-800"}`}
                  >
                    {p} meses
                  </button>
                ))}
              </div>
            </div>
            <div className="h-72 w-full sm:h-80">
              <ResponsiveContainer width="100%" height="100%">
                <BarChart data={dadosPorMes} barGap={8} margin={{ top: 8, right: 10, left: 0, bottom: 0 }}>
                  <XAxis dataKey="mes" tickLine={false} axisLine={false} tick={{ fill: "#64748b", fontSize: 12 }} />
                  <YAxis
                    tickFormatter={(valor) => `R$ ${valor}`}
                    width={75}
                    tickLine={false}
                    axisLine={false}
                    tick={{ fill: "#64748b", fontSize: 11 }}
                  />
                  <Tooltip content={<CustomTooltip />} />
                  <Legend wrapperStyle={{ fontSize: 12, paddingTop: 10 }} />
                  <Bar dataKey="receita" name="Receitas" fill="#87A878" radius={[5, 5, 0, 0]} />
                  <Bar dataKey="despesa" name="Despesas" fill="#C46A3A" radius={[5, 5, 0, 0]} />
                  <Bar dataKey="investimento" name="Investimentos" fill="#C8A45D" radius={[5, 5, 0, 0]} />
                  <Line
                    type="monotone"
                    dataKey="saldoAcumulado"
                    name="Saldo acumulado"
                    stroke="#5B3A29"
                    strokeWidth={3}
                    dot={{ r: 3, fill: "#5B3A29" }}
                    activeDot={{ r: 5 }}
                  />
                </BarChart>
              </ResponsiveContainer>
            </div>
          </section>
        </div>
        {/* Fatura e gráfico de cartões */}
        <section className="grid grid-cols-1">
          <article className="overflow-hidden rounded-2xl border border-[#D9C7AB] bg-white shadow-sm">
            {/* Cabeçalho */}
            <div className="bg-linear-to-br from-[#5B3A29] to-[#C46A3A] p-5 text-white sm:p-6">
              <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
                <div>
                  <p className="text-xs font-bold uppercase tracking-[0.16em] text-[#F4EBDD]">Cartões de crédito</p>

                  <h2 className="mt-1 text-2xl font-bold">Fatura do mês</h2>

                  <p className="mt-1 text-sm capitalize text-white/70">{mesFatura}</p>
                </div>

                <button
                  onClick={() => navigate("/transacao")}
                  className="inline-flex items-center justify-center rounded-xl bg-white px-4 py-2.5 text-sm font-bold text-[#5B3A29] shadow-sm transition hover:bg-[#EEF2E8] focus:outline-none focus:ring-4 focus:ring-white/20"
                >
                  Ver compras parceladas
                  <span className="ml-2">→</span>
                </button>
              </div>
            </div>

            <div className="space-y-5 p-5 sm:p-6">
              {/* Total de todos os cartões */}
              <div className="rounded-2xl bg-[#F4EBDD] p-5 sm:p-6">
                <div className="flex items-start justify-between gap-4">
                  <div className="min-w-0">
                    <p className="text-sm font-medium text-[#5B3A29]">Total gasto em todos os cartões neste mês</p>

                    <p className="mt-3 wrap-break-word text-3xl font-bold tracking-tight text-[#5B3A29] sm:text-4xl">
                      {loadingFaturas ? "Carregando..." : formatarValor(totalFaturaAtual)}
                    </p>

                    <p className="mt-2 text-sm text-[#5B3A29]/70">
                      {loadingFaturas
                        ? "Buscando faturas..."
                        : `${quantidadeItensFatura} ${quantidadeItensFatura === 1 ? "lançamento" : "lançamentos"} neste mês`}
                    </p>
                  </div>

                  {/* Período no canto superior direito */}
                  <div className="shrink-0 text-right">
                    <p className="text-xs font-bold uppercase tracking-wider text-[#5B3A29]/60">Período considerado</p>

                    <p className="mt-1 text-sm font-semibold text-[#5B3A29]">
                      {dayjs().startOf("month").format("DD/MM")} a {dayjs().endOf("month").format("DD/MM/YYYY")}
                    </p>
                  </div>
                </div>
                {/* Planejamento: abaixo dos indicadores e acima da fatura */}
                <section className="w-full">
                  <article className="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm sm:p-6">
                    <div className="mb-5">
                      <p className="text-xs font-bold uppercase tracking-[0.16em] text-[#87A878]">Planejamento</p>
                      <h2 className="mt-1 text-xl font-bold text-[#5B3A29]">Fatura total por mês</h2>
                    </div>
                    <div className="h-40 w-full sm:h-48">
                      <ResponsiveContainer width="100%" height="100%">
                        <BarChart data={historicoFaturas} margin={{ top: 8, right: 8, left: 0, bottom: 0 }}>
                          <XAxis dataKey="mes" tickLine={false} axisLine={false} tick={{ fill: "#64748b", fontSize: 12 }} />
                          <YAxis
                            tickFormatter={(valor) => `R$${valor}`}
                            width={62}
                            tickLine={false}
                            axisLine={false}
                            tick={{ fill: "#64748b", fontSize: 11 }}
                          />
                          <Tooltip formatter={(valor) => formatarValor(Number(valor))} contentStyle={{ borderRadius: 12, borderColor: "#D9C7AB" }} />
                          <Bar dataKey="total" name="Fatura total" fill="#87A878" radius={[6, 6, 0, 0]} maxBarSize={44} />
                        </BarChart>
                      </ResponsiveContainer>
                    </div>
                  </article>
                </section>
              </div>

              {/* Seletor de cartões */}
              <div>
                <div className="mb-3">
                  <h3 className="text-base font-bold text-[#5B3A29]">Seus cartões</h3>

                  <p className="mt-1 text-sm text-slate-500">Selecione um cartão para consultar seus lançamentos.</p>
                </div>

                {cartoes.length > 0 ? (
                  <div className="grid grid-cols-1 gap-3 sm:grid-cols-2 lg:grid-cols-3">
                    {cartoes.map((cartao) => {
                      const selecionado = cartaoSelecionadoId === cartao.id;

                      const faturaCartao = faturasPorCartao.find((fatura) => fatura.cartaoId === cartao.id);

                      return (
                        <button
                          key={cartao.id}
                          type="button"
                          onClick={() => setCartaoSelecionadoId(selecionado ? null : cartao.id)}
                          className={`flex items-center gap-3 rounded-xl border p-3 text-left transition ${
                            selecionado
                              ? "border-[#87A878] bg-[#EEF2E8] ring-2 ring-[#87A878]/30"
                              : "border-[#D9C7AB] bg-white hover:border-[#87A878] hover:bg-[#F9F6F0]"
                          }`}
                        >
                          {obterImagem(cartao.imagem) ? (
                            <img
                              src={obterImagem(cartao.imagem)}
                              alt={`Cartão ${cartao.nome}`}
                              className="h-12 w-16 shrink-0 rounded-lg object-contain"
                            />
                          ) : (
                            <div className="flex h-12 w-16 shrink-0 items-center justify-center rounded-lg bg-linear-to-br from-[#5B3A29] to-[#C46A3A] text-white">
                              <svg
                                xmlns="http://www.w3.org/2000/svg"
                                viewBox="0 0 24 24"
                                fill="none"
                                stroke="currentColor"
                                strokeWidth="1.5"
                                className="h-7 w-7"
                              >
                                <rect x="2" y="5" width="20" height="14" rx="2" />
                                <path d="M2 10h20" />
                              </svg>
                            </div>
                          )}

                          <div className="min-w-0 flex-1">
                            <p className="truncate font-bold text-[#5B3A29]">{cartao.nome}</p>

                            <p className="mt-1 text-sm text-slate-500">{formatarValor(faturaCartao?.total ?? 0)}</p>
                          </div>

                          <div
                            className={`flex h-5 w-5 shrink-0 items-center justify-center rounded-full border ${
                              selecionado ? "border-[#87A878] bg-[#87A878] text-white" : "border-[#D9C7AB]"
                            }`}
                          >
                            {selecionado && (
                              <svg
                                xmlns="http://www.w3.org/2000/svg"
                                viewBox="0 0 24 24"
                                fill="none"
                                stroke="currentColor"
                                strokeWidth="3"
                                className="h-3 w-3"
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
                  <p className="rounded-xl border border-dashed border-[#D9C7AB] p-5 text-center text-sm text-slate-500">Nenhum cartão cadastrado.</p>
                )}
              </div>

              {/* Lançamentos do cartão selecionado */}
              {cartaoSelecionadoId !== null && (
                <div className="mt-5 border-t border-[#D9C7AB]/60 pt-5">
                  {(() => {
                    const cartao = cartoes.find((item) => item.id === cartaoSelecionadoId);

                    const fatura = faturasPorCartao.find((item) => item.cartaoId === cartaoSelecionadoId);

                    if (!cartao) return null;

                    return (
                      <>
                        {/* Cabeçalho da fatura */}
                        <div className="mb-5 flex flex-col gap-4 rounded-2xl border border-[#D9C7AB]/60 bg-[#F4EBDD]/50 p-4 sm:flex-row sm:items-center sm:justify-between sm:p-5">
                          <div className="flex min-w-0 items-center gap-3">
                            <div className="flex h-12 w-12 shrink-0 items-center justify-center rounded-xl bg-white text-xl text-[#5B3A29] shadow-sm">
                              $
                            </div>

                            <div className="min-w-0">
                              <p className="text-xs font-bold uppercase tracking-[0.14em] text-[#87A878]">Extrato do cartão</p>

                              <h3 className="mt-1 wrap-break-word text-lg font-bold text-[#5B3A29]">{cartao.nome}</h3>

                              <p className="mt-1 text-sm text-[#5B3A29]/65">
                                {loadingFaturas
                                  ? "Atualizando lançamentos..."
                                  : `${fatura?.compras.length ?? 0} ${fatura?.compras.length === 1 ? "lançamento" : "lançamentos"} nesta fatura`}
                              </p>
                              <p className="mt-2 text-sm font-semibold text-[#5B3A29]">Vencimento: dia {cartao.diaVencimento}</p>
                            </div>
                          </div>

                          <div className="border-t border-[#D9C7AB]/60 pt-3 sm:border-l sm:border-t-0 sm:pl-5 sm:pt-0 sm:text-right">
                            <p className="text-xs font-medium text-[#5B3A29]/65">Total da fatura</p>

                            <p className="mt-1 text-2xl font-bold tracking-tight text-[#5B3A29]">{formatarValor(fatura?.total ?? 0)}</p>
                          </div>
                        </div>

                        {/* Lista de compras */}
                        {loadingFaturas ? (
                          <div className="space-y-3">
                            {[...Array(3)].map((_, index) => (
                              <div key={index} className="flex animate-pulse items-center gap-3 rounded-xl border border-slate-100 p-4">
                                <div className="h-10 w-10 shrink-0 rounded-xl bg-slate-200" />

                                <div className="flex-1 space-y-2">
                                  <div className="h-4 w-2/3 rounded bg-slate-200" />
                                  <div className="h-3 w-1/3 rounded bg-slate-100" />
                                </div>

                                <div className="h-4 w-20 rounded bg-slate-200" />
                              </div>
                            ))}
                          </div>
                        ) : fatura && fatura.compras.length > 0 ? (
                          <div className="space-y-2">
                            <div className="mb-3 flex items-center justify-between px-1">
                              <p className="text-sm font-semibold text-[#5B3A29]">Compras da fatura</p>

                              <p className="text-xs text-slate-400">
                                {fatura.compras.length} {fatura.compras.length === 1 ? "compra" : "compras"}
                              </p>
                            </div>

                            {fatura.compras.map((compra) => {
                              const isParcela = compra.origem === "parcela";

                              return (
                                <div
                                  key={`${compra.origem}-${compra.id}`}
                                  className="group flex flex-col gap-3 rounded-xl border border-[#E8DCC6]/70 bg-white p-4 transition duration-200 hover:border-[#C8A45D]/70 hover:bg-[#FDFBF7] sm:flex-row sm:items-center sm:justify-between"
                                >
                                  <div className="flex min-w-0 items-center gap-3">
                                    <div className={`h-10 w-1 shrink-0 rounded-full ${isParcela ? "bg-[#C46A3A]" : "bg-[#87A878]"}`} />

                                    <div className="min-w-0">
                                      <p className="wrap-break-word text-sm font-semibold text-[#5B3A29]">{compra.descricaoCompra}</p>

                                      <div className="mt-1.5 flex flex-wrap items-center gap-2">
                                        <span
                                          className={`rounded-md px-2 py-1 text-[10px] font-bold uppercase tracking-wide ${
                                            isParcela ? "bg-[#F8E9DF] text-[#A94F2B]" : "bg-[#EEF2E8] text-[#587B4C]"
                                          }`}
                                        >
                                          {compra.origem === "parcela" ? "Parcela" : "Compra à vista"} ·{" "}
                                        </span>

                                        <span className="text-xs text-slate-500">
                                          Data da compra: {dayjs(compra.dataCompra).format("DD/MM/YYYY")}
                                        </span>
                                      </div>
                                    </div>
                                  </div>

                                  <p className="pl-13 text-base font-bold text-[#5B3A29] sm:pl-4 sm:text-right">{formatarValor(compra.valor)}</p>
                                </div>
                              );
                            })}
                          </div>
                        ) : (
                          <div className="rounded-2xl border border-dashed border-[#D9C7AB] bg-[#F9F6F0] px-4 py-10 text-center">
                            <div className="mx-auto flex h-12 w-12 items-center justify-center rounded-xl bg-[#F4EBDD] text-xl text-[#5B3A29]">—</div>

                            <p className="mt-3 font-semibold text-[#5B3A29]">Nenhuma compra encontrada</p>

                            <p className="mx-auto mt-1 max-w-sm text-sm text-slate-500">Não há compras pendentes neste cartão para o mês atual.</p>
                          </div>
                        )}
                      </>
                    );
                  })()}
                </div>
              )}

              {!loadingFaturas && faturasPorCartao.length === 0 && (
                <p className="rounded-xl border border-dashed border-[#D9C7AB] px-4 py-5 text-center text-sm text-slate-500">
                  Nenhuma compra pendente nas faturas deste mês.
                </p>
              )}
            </div>
          </article>
        </section>

        {/* Transações recentes */}
        <section className="overflow-hidden rounded-2xl border border-[#D9C7AB]/70 bg-white shadow-sm">
          {/* Cabeçalho */}
          <div className="flex flex-col gap-4 border-b border-[#D9C7AB]/50 px-5 py-5 sm:flex-row sm:items-center sm:justify-between sm:px-6">
            <div>
              <div className="flex items-center gap-2">
                <span className="h-2 w-2 rounded-full bg-[#87A878]" />
                <p className="text-xs font-bold uppercase tracking-[0.16em] text-[#87A878]">Atividade recente</p>
              </div>

              <h2 className="mt-2 text-xl font-bold tracking-tight text-[#5B3A29] sm:text-2xl">Últimas transações</h2>

              <p className="mt-1 text-sm text-slate-500">Seus cinco lançamentos mais recentes nos filtros atuais.</p>
            </div>

            <button
              onClick={() => navigate("/transacao")}
              className="inline-flex items-center justify-center gap-2 self-start rounded-xl border border-[#D9C7AB] bg-[#F4EBDD]/50 px-4 py-2.5 text-sm font-bold text-[#5B3A29] transition duration-200 hover:border-[#C8A45D] hover:bg-[#F4EBDD] focus:outline-none focus:ring-4 focus:ring-[#87A878]/20 sm:self-auto"
            >
              Ver todas
              <span className="text-base transition-transform duration-200 group-hover:translate-x-1">→</span>
            </button>
          </div>

          {/* Conteúdo */}
          <div className="p-4 sm:p-6">
            {loading ? (
              <div className="space-y-3">
                {[...Array(4)].map((_, i) => (
                  <div key={i} className="flex animate-pulse items-center gap-4 rounded-xl border border-slate-100 p-4">
                    <div className="h-11 w-11 shrink-0 rounded-xl bg-slate-200" />

                    <div className="flex-1 space-y-2">
                      <div className="h-4 w-1/2 rounded bg-slate-200" />
                      <div className="h-3 w-1/4 rounded bg-slate-100" />
                    </div>

                    <div className="h-5 w-24 rounded bg-slate-200" />
                  </div>
                ))}
              </div>
            ) : ultimasTransacoes.length === 0 ? (
              <div className="rounded-xl border border-dashed border-[#D9C7AB] bg-[#F4EBDD]/30 px-4 py-12 text-center">
                <div className="mx-auto flex h-12 w-12 items-center justify-center rounded-2xl bg-[#F4EBDD] text-2xl text-[#5B3A29]">—</div>

                <p className="mt-4 font-semibold text-[#5B3A29]">Nenhuma transação encontrada</p>

                <p className="mx-auto mt-1 max-w-sm text-sm text-slate-500">Não encontramos lançamentos para os filtros selecionados.</p>

                {filtrosAtivos && (
                  <button
                    onClick={limparFiltros}
                    className="mt-4 rounded-lg px-4 py-2 text-sm font-bold text-[#5B3A29] transition hover:bg-[#EEF2E8]"
                  >
                    Limpar filtros
                  </button>
                )}
              </div>
            ) : (
              <div className={`space-y-3 transition-all duration-200 ${animando ? "translate-y-1 opacity-60" : "opacity-100"}`}>
                {ultimasTransacoes.map((t) => {
                  const isEntrada = t.tipo === 1;
                  const isMaiorTransacao = t.id === maiorTransacaoId;

                  return (
                    <article
                      key={t.id}
                      className={`group flex flex-col gap-3 rounded-xl border p-4 transition duration-200 hover:-translate-y-px hover:shadow-sm sm:flex-row sm:items-center sm:justify-between ${
                        isMaiorTransacao
                          ? "border-[#B8CDAA] bg-[#EEF2E8]/40 hover:border-[#87A878]"
                          : "border-slate-100 bg-white hover:border-[#D9C7AB]"
                      }`}
                    >
                      <div className="flex min-w-0 items-center gap-3">
                        <div
                          className={`flex h-11 w-11 shrink-0 items-center justify-center rounded-xl text-xl font-bold ${
                            isEntrada ? "bg-[#EEF2E8] text-[#587B4C]" : "bg-[#F8E9DF] text-[#A94F2B]"
                          }`}
                          aria-label={isEntrada ? "Entrada" : "Saída"}
                        >
                          {isEntrada ? "↙" : "↗"}
                        </div>

                        <div className="min-w-0">
                          <p className="wrap-break-word font-semibold text-[#5B3A29]">{t.descricao}</p>

                          <div className="mt-1 flex flex-wrap items-center gap-2 text-xs text-slate-500 sm:text-sm">
                            <span>{dayjs(t.data).isValid() ? dayjs(t.data).format("DD/MM/YYYY") : t.data}</span>

                            <span className="h-1 w-1 rounded-full bg-slate-300" />

                            <span className={`font-medium ${isEntrada ? "text-[#587B4C]" : "text-[#A94F2B]"}`}>
                              {isEntrada ? "Entrada" : "Saída"}
                            </span>

                            {isMaiorTransacao && (
                              <>
                                <span className="h-1 w-1 rounded-full bg-slate-300" />

                                <span className="rounded-md bg-[#DDE8D5] px-2 py-0.5 text-[10px] font-bold uppercase tracking-wide text-[#587B4C]">
                                  Maior valor
                                </span>
                              </>
                            )}
                          </div>
                        </div>
                      </div>

                      <p
                        className={`pl-14 text-lg font-bold tracking-tight sm:pl-4 sm:text-right ${isEntrada ? "text-[#587B4C]" : "text-[#A94F2B]"}`}
                      >
                        {isEntrada ? "+" : "−"} {formatarValor(t.valor)}
                      </p>
                    </article>
                  );
                })}
              </div>
            )}
          </div>
        </section>

        {!loading && resumoPorUsuario.length === 0 && (
          <div className="rounded-xl border border-dashed border-slate-300 p-5 text-center text-sm text-slate-500">
            Não há lançamentos para os filtros selecionados.
          </div>
        )}
      </div>
    </main>
  );
}
