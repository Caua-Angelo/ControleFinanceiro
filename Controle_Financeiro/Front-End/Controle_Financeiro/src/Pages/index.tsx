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

  const transacoesPeriodo = useMemo(
    () => filterByPeriod(tipoSelecionado === "todos" ? transacoes : transacoes.filter((t) => t.tipo === Number(tipoSelecionado)), period),

    [transacoes, tipoSelecionado, period],
  );

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

  const totalGeralDespesas = useMemo(() => resumoPorUsuario.reduce((acc, curr) => acc + curr.totalDespesas, 0), [resumoPorUsuario]);

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

      if (!mapa.has(chave)) mapa.set(chave, { receita: 0, despesa: 0, investimento: 0 });

      return mapa.get(chave)!;
    };

    transacoesPeriodo.forEach((t) => {
      const data = dayjs(t.data, ["YYYY-MM-DD", "DD/MM/YYYY"]);

      if (!data.isValid()) return;

      const item = garantirMes(data);

      if (t.finalidadeCategoria === Finalidade.Investimento) item.investimento += t.valor;
      else if (t.tipo === 1) item.receita += t.valor;
      else if (t.tipo === 2) item.despesa += t.valor;
    }); // Cada parcela entra no mês da fatura: se a compra foi feita depois do fechamento,
    // a primeira parcela passa para o mês seguinte; as demais seguem mês a mês.

    comprasParceladas.forEach((compra) => {
      const compraComDados = compra as CompraParceladaResponse & {
        dataCompra?: string;

        cartaoId?: number;

        parcelas: Array<{ valor: number; dataVencimento?: string; paga?: boolean }>;
      };

      const dataCompra = dayjs(compraComDados.dataCompra);

      if (!dataCompra.isValid()) return;

      const cartaoId = (compra as CompraParceladaResponse & { cartaoId?: number }).cartaoId;

      const cartao = cartoes.find((c) => c.id === cartaoId);

      const diaFechamento = cartao?.diaFechamento ?? 31;

      const passouFechamento = dataCompra.date() > diaFechamento;

      compraComDados.parcelas.forEach((parcela, indice) => {
        // Distribui uma parcela por mês, começando no mês da compra; se a compra

        // ocorreu após o fechamento, a primeira parcela entra no mês seguinte.

        const mesParcela = dataCompra.startOf("month").add(indice + (passouFechamento ? 1 : 0), "month");

        const inicioPeriodo = dayjs()
          .subtract(period - 1, "month")

          .startOf("month");

        if (mesParcela.isBefore(inicioPeriodo, "month") || mesParcela.isAfter(dayjs().add(4, "month"), "month")) return;

        garantirMes(mesParcela).despesa += parcela.valor;
      });
    });

    const dadosOrdenados = Array.from({ length: period + 4 }).map((_, i) => {
      const data = dayjs()
        .subtract(period - 1, "month")
        .add(i, "month");

      const chave = data.format("MM/YY");

      const valores = mapa.get(chave) || { receita: 0, despesa: 0, investimento: 0 };

      return { mes: chave, receita: valores.receita, despesa: valores.despesa, investimento: valores.investimento };
    });

    let saldo = 0;

    return dadosOrdenados.map((item) => {
      saldo += item.receita - item.despesa - item.investimento;

      return { ...item, saldoAcumulado: saldo };
    });
  }, [transacoesPeriodo, comprasParceladas, cartoes, period]);

  function filterByPeriod(transactions: TransacaoResponse[], months: number) {
    const startDate = dayjs()
      .subtract(months - 1, "month")

      .startOf("month");

    return transactions.filter((t) => {
      const data = dayjs(t.data).startOf("day");

      return data.isValid() && (data.isSame(startDate, "month") || data.isAfter(startDate, "month"));
    });
  }

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
      <div className="mx-auto w-full max-w-[1600px] space-y-5">
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
          <div className="mt-5 flex flex-wrap items-center justify-between gap-3 border-t border-slate-100 pt-4">
            <p className="text-xs text-slate-500">
              {filtrosAtivos ? "Resumo atualizado conforme os filtros selecionados." : "Você está vendo todos os lançamentos."}
            </p>
            <button
              onClick={limparFiltros}
              disabled={!filtrosAtivos}
              className="rounded-lg px-3 py-2 text-xs font-bold text-[#5B3A29] transition hover:bg-[#EEF2E8] disabled:cursor-not-allowed disabled:text-slate-300"
            >
              Limpar filtros ↺
            </button>
          </div>
        </section>

        {/* Indicadores */}
        <section className="grid grid-cols-1 gap-3 sm:grid-cols-2 2xl:grid-cols-4">
          <article className="relative overflow-hidden rounded-2xl border border-[#D5E2CC] bg-white p-5 shadow-sm transition hover:-translate-y-0.5 hover:shadow-md">
            <div className="absolute right-0 top-0 h-20 w-20 rounded-bl-full bg-[#EEF2E8]" />
            <div className="relative flex items-center gap-3">
              <span className="flex h-11 w-11 items-center justify-center rounded-xl bg-[#EEF2E8] text-xl font-bold text-[#587B4C]">↙</span>
              <div>
                <p className="text-sm font-medium text-slate-500">Receitas</p>
                <p className="mt-1 text-xs text-[#587B4C]">Entradas no período</p>
              </div>
            </div>
            <p className="relative mt-5 break-words text-2xl font-bold tracking-tight text-[#587B4C] sm:text-3xl">
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
            <p className="relative mt-5 break-words text-2xl font-bold tracking-tight text-[#A94F2B] sm:text-3xl">
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
            <p className="relative mt-5 break-words text-2xl font-bold tracking-tight text-[#94732F] sm:text-3xl">
              {formatarValor(totalGeralInvestimentos)}
            </p>
          </article>
          <article className="relative overflow-hidden rounded-2xl border border-[#D9C7AB] bg-[#5B3A29] p-5 shadow-sm transition hover:-translate-y-0.5 hover:shadow-md">
            <div className="absolute -right-4 -top-8 h-28 w-28 rounded-full border-[18px] border-white/5" />
            <div className="relative flex items-center gap-3">
              <span className="flex h-11 w-11 items-center justify-center rounded-xl bg-white/10 text-xl font-bold text-white">=</span>
              <div>
                <p className="text-sm font-medium text-white/75">Saldo final</p>
                <p className="mt-1 text-xs text-white/60">Receitas menos saídas</p>
              </div>
            </div>
            <p
              className={`relative mt-5 break-words text-2xl font-bold tracking-tight sm:text-3xl ${saldoGeralFinal >= 0 ? "text-white" : "text-[#F0C8AE]"}`}
            >
              {formatarValor(saldoGeralFinal)}
            </p>
          </article>
        </section>

        {/* Fatura e gráfico de cartões */}
        <section className="grid grid-cols-1 items-stretch gap-5 2xl:grid-cols-[1.05fr_1fr]">
          <article className="overflow-hidden rounded-2xl border border-[#D9C7AB] bg-white shadow-sm">
            <div className="flex flex-col gap-4 bg-gradient-to-br from-[#5B3A29] to-[#C46A3A] p-5 text-white sm:flex-row sm:items-center sm:justify-between sm:p-6">
              <div>
                <p className="text-xs font-bold uppercase tracking-[0.16em] text-[#F4EBDD]">Cartão de crédito</p>
                <h2 className="mt-1 text-2xl font-bold">Fatura em aberto</h2>
                <p className="mt-1 text-sm capitalize text-white/70">{mesFatura}</p>
              </div>
              <button
                onClick={() => navigate("/transacao")}
                className="inline-flex items-center justify-center rounded-xl bg-white px-4 py-2.5 text-sm font-bold text-[#5B3A29] shadow-sm transition hover:bg-[#EEF2E8] focus:outline-none focus:ring-4 focus:ring-white/20"
              >
                Ver compras parceladas <span className="ml-2">→</span>
              </button>
            </div>
            <div className="grid grid-cols-1 gap-4 p-5 sm:grid-cols-[1.2fr_1fr] sm:p-6">
              <div className="rounded-2xl bg-[#F4EBDD] p-5">
                <p className="text-sm font-medium text-[#5B3A29]">Total previsto neste mês</p>
                <p className="mt-3 break-words text-3xl font-bold tracking-tight text-[#5B3A29] sm:text-4xl">
                  {loadingFaturas ? "Carregando..." : formatarValor(totalFaturaAtual)}
                </p>
                <p className="mt-2 text-sm text-slate-500">
                  {loadingFaturas
                    ? "Buscando parcelas..."
                    : `${quantidadeItensFatura} ${quantidadeItensFatura === 1 ? "lançamento pendente" : "lançamentos pendentes"}`}
                </p>
              </div>
              <div className="flex flex-col justify-center rounded-2xl border border-slate-200 p-5">
                <p className="text-xs font-bold uppercase tracking-wider text-slate-400">Período considerado</p>
                <p className="mt-2 font-semibold text-slate-800">
                  {dayjs().startOf("month").format("DD/MM")} a {dayjs().endOf("month").format("DD/MM/YYYY")}
                </p>
                <p className="mt-3 text-xs leading-5 text-slate-500">
                  Inclui parcelas pendentes e compras à vista no cartão, considerando o fechamento de cada cartão.
                </p>
              </div>
            </div>
            {!loadingFaturas && faturasPorCartao.length > 0 && (
              <div className="space-y-3 border-t border-slate-100 p-5 sm:px-6">
                {faturasPorCartao.map((fatura) => (
                  <div key={fatura.cartaoId} className="rounded-xl border border-slate-200 p-4">
                    <div className="mb-3 flex flex-wrap items-center justify-between gap-2">
                      <h3 className="font-bold text-slate-800">{fatura.cartaoNome}</h3>
                      <p className="text-lg font-bold text-[#5B3A29]">{formatarValor(fatura.total)}</p>
                    </div>
                    <div className="space-y-2">
                      {fatura.compras.map((compra) => (
                        <div
                          key={`${compra.origem}-${compra.id}`}
                          className="flex flex-col gap-1 rounded-lg bg-slate-50 px-3 py-3 sm:flex-row sm:items-center sm:justify-between"
                        >
                          <div className="min-w-0">
                            <p className="break-words text-sm font-semibold text-slate-800">{compra.descricaoCompra}</p>
                            <p className="mt-1 text-xs text-slate-500">
                              {compra.origem === "parcela" ? "Parcela" : "Compra à vista"} · {dayjs(compra.dataVencimento).format("DD/MM/YYYY")}
                            </p>
                          </div>
                          <p className="text-sm font-bold text-slate-700">{formatarValor(compra.valor)}</p>
                        </div>
                      ))}
                    </div>
                  </div>
                ))}
              </div>
            )}
            {!loadingFaturas && faturasPorCartao.length === 0 && (
              <div className="border-t border-slate-100 px-5 py-6 text-sm text-slate-500">Nenhuma compra pendente na fatura deste mês.</div>
            )}
          </article>

          <article className="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm sm:p-6">
            <div className="mb-5">
              <p className="text-xs font-bold uppercase tracking-[0.16em] text-[#87A878]">Planejamento</p>
              <h2 className="mt-1 text-xl font-bold text-[#5B3A29]">Fatura total por mês</h2>
              <p className="mt-1 text-sm leading-5 text-slate-500">Veja como os compromissos do cartão se distribuem ao longo dos meses.</p>
            </div>
            <div className="h-64 w-full sm:h-72">
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

        {/* Evolução financeira */}
        <section className="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm sm:p-6">
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

        {/* Transações recentes */}
        <section className="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm sm:p-6">
          <div className="mb-5 flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
            <div>
              <p className="text-xs font-bold uppercase tracking-[0.16em] text-[#87A878]">Atividade recente</p>
              <h2 className="mt-1 text-xl font-bold text-[#5B3A29] sm:text-2xl">Últimas transações</h2>
              <p className="mt-1 text-sm text-slate-500">Os cinco lançamentos mais recentes dentro dos filtros atuais.</p>
            </div>
            <button
              onClick={() => navigate("/transacao")}
              className="inline-flex items-center justify-center rounded-xl border border-[#D9C7AB] px-4 py-2.5 text-sm font-bold text-[#5B3A29] transition hover:bg-[#F4EBDD] focus:outline-none focus:ring-4 focus:ring-[#87A878]/20"
            >
              Ver todas <span className="ml-2">→</span>
            </button>
          </div>
          {loading ? (
            <div className="space-y-3">
              {[...Array(4)].map((_, i) => (
                <div key={i} className="animate-pulse rounded-xl border border-slate-100 p-4">
                  <div className="mb-2 h-4 w-1/2 rounded bg-slate-200" />
                  <div className="h-3 w-1/3 rounded bg-slate-100" />
                </div>
              ))}
            </div>
          ) : ultimasTransacoes.length === 0 ? (
            <div className="rounded-xl border border-dashed border-slate-300 px-4 py-10 text-center">
              <p className="font-semibold text-slate-700">Nenhuma transação encontrada</p>
              <p className="mt-1 text-sm text-slate-500">Tente limpar os filtros para visualizar outros lançamentos.</p>
              {filtrosAtivos && (
                <button onClick={limparFiltros} className="mt-3 rounded-lg px-3 py-2 text-sm font-bold text-[#5B3A29] hover:bg-[#EEF2E8]">
                  Limpar filtros
                </button>
              )}
            </div>
          ) : (
            <div className={`space-y-2 transition-all duration-200 ${animando ? "translate-y-1 opacity-60" : "opacity-100"}`}>
              {ultimasTransacoes.map((t) => (
                <div
                  key={t.id}
                  className={`flex flex-col gap-3 rounded-xl border p-4 transition hover:border-slate-300 sm:flex-row sm:items-center sm:justify-between ${t.id === maiorTransacaoId ? "border-[#B8CDAA] bg-[#EEF2E8]/50" : "border-slate-100 bg-white"}`}
                >
                  <div className="flex min-w-0 items-start gap-3">
                    <span
                      className={`flex h-11 w-11 shrink-0 items-center justify-center rounded-xl text-lg font-bold ${t.tipo === 1 ? "bg-[#EEF2E8] text-[#587B4C]" : "bg-[#F8E9DF] text-[#A94F2B]"}`}
                      aria-hidden="true"
                    >
                      {t.tipo === 1 ? "↙" : "↗"}
                    </span>
                    <div className="min-w-0">
                      <p className="break-words font-semibold text-slate-800">{t.descricao}</p>
                      <p className="mt-1 text-sm text-slate-500">{dayjs(t.data).isValid() ? dayjs(t.data).format("DD/MM/YYYY") : t.data}</p>
                    </div>
                  </div>
                  <p className={`pl-14 text-lg font-bold sm:pl-0 sm:text-right ${t.tipo === 1 ? "text-[#587B4C]" : "text-[#A94F2B]"}`}>
                    {t.tipo === 1 ? "+" : "−"} {formatarValor(t.valor)}
                  </p>
                </div>
              ))}
            </div>
          )}
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
