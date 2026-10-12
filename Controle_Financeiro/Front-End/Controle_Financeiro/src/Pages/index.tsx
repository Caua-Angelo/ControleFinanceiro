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
import type { TransacaoResponse, UsuarioResumoResponse } from "../Types/index";
import type { CompraParceladaResponse } from "../Types/CompraParceladaResponse";
import { CustomTooltip } from "../Components/CustomTooltip";

dayjs.extend(customParseFormat);

export default function RelatorioFinanceiro() {
  const navigate = useNavigate();
  const [transacoes, setTransacoes] = useState<TransacaoResponse[]>([]);
  const [comprasParceladas, setComprasParceladas] = useState<CompraParceladaResponse[]>([]);
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
        const [resultadoTransacoes, resultadoCompras] = await Promise.allSettled([listarTransacoes(), listarComprasParceladas()]);

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
    const usuariosMap = new Map<number, UsuarioResumoResponse>();

    transacoesFiltradas.forEach((transacao) => {
      if (!usuariosMap.has(transacao.usuarioId)) {
        usuariosMap.set(transacao.usuarioId, {
          usuarioId: transacao.usuarioId,
          usuarioNome: transacao.usuarioNome,
          totalReceitas: 0,
          totalDespesas: 0,
          saldoFinal: 0,
        });
      }

      const usuario = usuariosMap.get(transacao.usuarioId)!;
      if (transacao.tipo === 1) usuario.totalReceitas += transacao.valor;
      else usuario.totalDespesas += transacao.valor;
      usuario.saldoFinal = usuario.totalReceitas - usuario.totalDespesas;
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
  const saldoGeralFinal = totalGeralReceitas - totalGeralDespesas;

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
    const mapa = new Map<string, { receita: number; despesa: number }>();

    transacoesPeriodo.forEach((t) => {
      const data = dayjs(t.data, ["YYYY-MM-DD", "DD/MM/YYYY"]);
      if (!data.isValid()) return;
      const chave = data.format("MM/YY");
      if (!mapa.has(chave)) mapa.set(chave, { receita: 0, despesa: 0 });

      const item = mapa.get(chave)!;
      if (t.tipo === 1) item.receita += t.valor;
      else item.despesa += t.valor;
    });

    const dadosOrdenados = Array.from({ length: period }).map((_, i) => {
      const data = dayjs().subtract(period - 1 - i, "month");
      const chave = data.format("MM/YY");
      const valores = mapa.get(chave) || { receita: 0, despesa: 0 };
      return { mes: chave, receita: valores.receita, despesa: valores.despesa };
    });

    let saldo = 0;
    return dadosOrdenados.map((item) => {
      saldo += item.receita - item.despesa;
      return { ...item, saldoAcumulado: saldo };
    });
  }, [transacoesPeriodo, period]);

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

  const faturaAtual = useMemo(() => {
    const inicioMes = dayjs().startOf("month");
    const fimMes = dayjs().endOf("month");
    const parcelasPendentes = comprasParceladas.flatMap((compra) =>
      compra.parcelas
        .filter((parcela) => {
          const vencimento = dayjs(parcela.dataVencimento);
          return (
            !parcela.paga &&
            vencimento.isValid() &&
            (vencimento.isAfter(inicioMes.subtract(1, "day")) || vencimento.isSame(inicioMes, "day")) &&
            (vencimento.isBefore(fimMes.add(1, "day")) || vencimento.isSame(fimMes, "day"))
          );
        })
        .map((parcela) => ({ ...parcela, descricaoCompra: compra.descricao })),
    );

    return {
      total: parcelasPendentes.reduce((total, parcela) => total + parcela.valor, 0),
      quantidade: parcelasPendentes.length,
      parcelas: parcelasPendentes,
    };
  }, [comprasParceladas]);

  dayjs.locale("pt-br");

  const mesFatura = dayjs().format("MMMM [de] YYYY");

  return (
    <div className="w-full max-w-full space-y-5 overflow-x-hidden pb-6">
      <section className="rounded-2xl border border-emerald-900/10 bg-linear-to-br from-[#F5F8F6] to-white p-4 shadow-sm sm:p-6">
        <div className="mb-5 flex flex-col gap-4 lg:flex-row lg:items-end lg:justify-between">
          <div>
            <p className="mb-1 text-sm font-medium uppercase tracking-wider text-[#658176]">Visão financeira</p>
            <h1 className="text-2xl font-bold tracking-tight text-[#24483E] sm:text-3xl">Resumo geral</h1>
            <p className="mt-1 text-sm text-slate-500">Acompanhe suas receitas, despesas e saldo em um só lugar.</p>
          </div>

          <div className="grid grid-cols-1 gap-3 sm:grid-cols-2">
            <label className="flex flex-col gap-1.5 text-sm font-medium text-slate-600">
              Mês
              <select
                id="select-mes"
                value={mesSelecionado}
                onChange={(e) => setMesSelecionado(e.target.value)}
                className="min-w-44 rounded-xl border border-slate-200 bg-white px-3 py-2.5 text-slate-800 outline-none transition focus:border-emerald-700 focus:ring-4 focus:ring-emerald-700/10"
              >
                <option value="todos">Todos os meses</option>
                {gerarMeses().map((mes) => (
                  <option key={mes.valor} value={mes.valor}>
                    {mes.label.charAt(0).toUpperCase() + mes.label.slice(1)}
                  </option>
                ))}
              </select>
            </label>

            <label className="flex flex-col gap-1.5 text-sm font-medium text-slate-600">
              Tipo de lançamento
              <select
                id="select-tipo"
                value={tipoSelecionado}
                onChange={(e) => setTipoSelecionado(e.target.value === "todos" ? "todos" : Number(e.target.value))}
                className="min-w-40 rounded-xl border border-slate-200 bg-white px-3 py-2.5 text-slate-800 outline-none transition focus:border-emerald-700 focus:ring-4 focus:ring-emerald-700/10"
              >
                <option value="todos">Todos os tipos</option>
                <option value={1}>Receitas</option>
                <option value={2}>Despesas</option>
              </select>
            </label>
          </div>
        </div>

        <div className="flex flex-wrap items-center justify-between gap-3 border-t border-slate-200 pt-4">
          <p className="text-sm text-slate-500">
            {filtrosAtivos ? "Exibindo resultados conforme os filtros selecionados." : "Exibindo todos os lançamentos."}
          </p>
          <button
            onClick={limparFiltros}
            disabled={!filtrosAtivos}
            className="rounded-lg px-3 py-2 text-sm font-semibold text-[#2F5D50] transition hover:bg-emerald-900/5 disabled:cursor-not-allowed disabled:text-slate-400"
          >
            Limpar filtros
          </button>
        </div>

        <div className="mt-5 grid grid-cols-1 gap-3 sm:grid-cols-2 xl:grid-cols-3">
          <div className="rounded-2xl border border-emerald-100 bg-white p-5 shadow-sm">
            <div className="mb-3 flex items-center gap-3">
              <span className="flex h-10 w-10 items-center justify-center rounded-xl bg-emerald-50 text-lg text-emerald-700">↗</span>
              <p className="text-sm font-medium text-slate-500">Receitas</p>
            </div>
            <p className="wrap-break-word text-2xl font-bold tracking-tight text-emerald-700 sm:text-3xl">{formatarValor(totalGeralReceitas)}</p>
          </div>

          <div className="rounded-2xl border border-rose-100 bg-white p-5 shadow-sm">
            <div className="mb-3 flex items-center gap-3">
              <span className="flex h-10 w-10 items-center justify-center rounded-xl bg-rose-50 text-lg text-rose-700">↘</span>
              <p className="text-sm font-medium text-slate-500">Despesas</p>
            </div>
            <p className="wrap-break-word text-2xl font-bold tracking-tight text-rose-700 sm:text-3xl">{formatarValor(totalGeralDespesas)}</p>
          </div>

          <div className="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm sm:col-span-2 xl:col-span-1">
            <div className="mb-3 flex items-center gap-3">
              <span className="flex h-10 w-10 items-center justify-center rounded-xl bg-slate-100 text-lg text-slate-700">=</span>
              <p className="text-sm font-medium text-slate-500">Saldo final</p>
            </div>
            <p className={`wrap-break-word text-2xl font-bold tracking-tight sm:text-3xl ${saldoGeralFinal >= 0 ? "text-emerald-700" : "text-rose-700"}`}>
              {formatarValor(saldoGeralFinal)}
            </p>
          </div>
        </div>
      </section>

      <section className="overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-sm">
        <div className="flex flex-col gap-4 border-b border-slate-100 bg-slate-50/70 p-4 sm:flex-row sm:items-center sm:justify-between sm:p-5">
          <div>
            <p className="text-xs font-semibold uppercase tracking-wider text-[#658176]">Cartão de crédito</p>
            <h2 className="mt-1 text-xl font-bold text-[#24483E] sm:text-2xl">Fatura em aberto</h2>
            <p className="mt-1 text-sm capitalize text-slate-500">{mesFatura}</p>
          </div>
          <button
            onClick={() => navigate("/transacao")}
            className="inline-flex items-center justify-center rounded-xl bg-[#285748] px-4 py-2.5 text-sm font-semibold text-white shadow-sm transition hover:bg-[#1F4639] focus:outline-none focus:ring-4 focus:ring-emerald-800/20"
          >
            Ver compras parceladas{" "}
            <span className="ml-2" aria-hidden="true">
              →
            </span>
          </button>
        </div>

        <div className="grid grid-cols-1 gap-4 p-4 sm:grid-cols-3 sm:p-5">
          <div className="rounded-xl bg-[#F1F7F4] p-4 sm:col-span-2">
            <p className="text-sm font-medium text-[#527165]">Total de parcelas pendentes com vencimento neste mês</p>
            <p className="mt-2 wrap-break-word text-3xl font-bold tracking-tight text-[#24483E] sm:text-4xl">
              {loadingFaturas ? "Carregando..." : formatarValor(faturaAtual.total)}
            </p>
            <p className="mt-2 text-sm text-slate-500">
              {loadingFaturas
                ? "Buscando parcelas..."
                : `${faturaAtual.quantidade} ${faturaAtual.quantidade === 1 ? "parcela pendente" : "parcelas pendentes"}`}
            </p>
          </div>
          <div className="flex flex-col justify-center rounded-xl border border-slate-200 p-4">
            <p className="text-sm text-slate-500">Período considerado</p>
            <p className="mt-1 font-semibold capitalize text-slate-800">
              {dayjs().startOf("month").format("DD/MM")} a {dayjs().endOf("month").format("DD/MM/YYYY")}
            </p>
            <p className="mt-2 text-xs leading-relaxed text-slate-500">
              O valor considera apenas parcelas ainda não marcadas como pagas, com vencimento neste mês.
            </p>
          </div>
        </div>

        {!loadingFaturas && faturaAtual.parcelas.length > 0 && (
          <div className="border-t border-slate-100 px-4 py-4 sm:px-5">
            <h3 className="mb-3 text-sm font-semibold text-slate-700">Parcelas incluídas na fatura</h3>
            <div className="space-y-2">
              {faturaAtual.parcelas.slice(0, 4).map((parcela) => (
                <div
                  key={parcela.id}
                  className="flex flex-col gap-1 rounded-xl border border-slate-100 px-3 py-3 sm:flex-row sm:items-center sm:justify-between"
                >
                  <div className="min-w-0">
                    <p className="truncate font-medium text-slate-800">{parcela.descricaoCompra}</p>
                    <p className="text-xs text-slate-500">Vencimento em {dayjs(parcela.dataVencimento).format("DD/MM/YYYY")}</p>
                  </div>
                  <p className="font-semibold text-slate-800">{formatarValor(parcela.valor)}</p>
                </div>
              ))}
              {faturaAtual.parcelas.length > 4 && (
                <p className="pt-1 text-xs text-slate-500">E mais {faturaAtual.parcelas.length - 4} parcelas nesta fatura.</p>
              )}
            </div>
          </div>
        )}

        {!loadingFaturas && faturaAtual.parcelas.length === 0 && (
          <div className="border-t border-slate-100 px-4 py-5 text-sm text-slate-500 sm:px-5">Nenhuma parcela pendente com vencimento neste mês.</div>
        )}
      </section>

      <section className="rounded-2xl border border-slate-200 bg-white p-4 shadow-sm sm:p-5">
        <div className="mb-5 flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
          <div>
            <p className="text-xs font-semibold uppercase tracking-wider text-[#658176]">Histórico</p>
            <h2 className="mt-1 text-xl font-bold text-[#24483E] sm:text-2xl">Evolução financeira</h2>
            <p className="mt-1 text-sm text-slate-500">Compare receitas, despesas e saldo acumulado ao longo do tempo.</p>
          </div>
          <div className="flex rounded-xl bg-slate-100 p-1" aria-label="Período do gráfico">
            {[3, 6, 12].map((p) => (
              <button
                key={p}
                onClick={() => setPeriod(p as 3 | 6 | 12)}
                aria-pressed={period === p}
                className={`rounded-lg px-3 py-2 text-sm font-semibold transition ${period === p ? "bg-white text-[#285748] shadow-sm" : "text-slate-500 hover:text-slate-800"}`}
              >
                {p} meses
              </button>
            ))}
          </div>
        </div>

        <div className="h-72 w-full sm:h-80">
          <ResponsiveContainer width="100%" height="100%">
            <BarChart data={dadosPorMes} barGap={8}>
              <XAxis dataKey="mes" tickLine={false} axisLine={false} />
              <YAxis tickFormatter={(valor) => `R$ ${valor}`} width={75} tickLine={false} axisLine={false} />
              <Tooltip content={<CustomTooltip />} />
              <Legend />
              <Bar dataKey="receita" name="Receitas" fill="#16a34a" radius={[5, 5, 0, 0]} />
              <Bar dataKey="despesa" name="Despesas" fill="#dc2626" radius={[5, 5, 0, 0]} />
              <Line type="linear" dataKey="saldoAcumulado" name="Saldo acumulado" stroke="#2563eb" strokeWidth={3} dot={{ r: 3 }} />
            </BarChart>
          </ResponsiveContainer>
        </div>
      </section>

      <section className="rounded-2xl border border-slate-200 bg-white p-4 shadow-sm sm:p-5">
        <div className="mb-4 flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
          <div>
            <p className="text-xs font-semibold uppercase tracking-wider text-[#658176]">Movimentações recentes</p>
            <h2 className="mt-1 text-xl font-bold text-[#24483E] sm:text-2xl">Últimas transações</h2>
          </div>
          <button
            onClick={() => navigate("/transacao")}
            className="inline-flex items-center justify-center rounded-xl border border-slate-200 px-4 py-2.5 text-sm font-semibold text-[#285748] transition hover:bg-slate-50 focus:outline-none focus:ring-4 focus:ring-emerald-800/10"
          >
            Ver todas{" "}
            <span className="ml-2" aria-hidden="true">
              →
            </span>
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
              <button onClick={limparFiltros} className="mt-3 rounded-lg px-3 py-2 text-sm font-semibold text-[#285748] hover:bg-emerald-50">
                Limpar filtros
              </button>
            )}
          </div>
        ) : (
          <div className={`space-y-2 transition-all duration-200 ${animando ? "translate-y-1 opacity-60" : "opacity-100"}`}>
            {ultimasTransacoes.map((t) => (
              <div
                key={t.id}
                className={`flex flex-col gap-3 rounded-xl border p-4 transition hover:border-slate-300 sm:flex-row sm:items-center sm:justify-between ${t.id === maiorTransacaoId ? "border-emerald-200 bg-emerald-50/50" : "border-slate-100 bg-white"}`}
              >
                <div className="flex min-w-0 items-start gap-3">
                  <span
                    className={`flex h-10 w-10 shrink-0 items-center justify-center rounded-xl text-lg ${t.tipo === 1 ? "bg-emerald-50 text-emerald-700" : "bg-rose-50 text-rose-700"}`}
                    aria-hidden="true"
                  >
                    {t.tipo === 1 ? "↗" : "↘"}
                  </span>
                  <div className="min-w-0">
                    <p className="wrap-break-word font-semibold text-slate-800">{t.descricao}</p>
                    <p className="mt-1 text-sm text-slate-500">{dayjs(t.data).isValid() ? dayjs(t.data).format("DD/MM/YYYY") : t.data}</p>
                  </div>
                </div>
                <p className={`pl-13 text-lg font-bold sm:pl-0 sm:text-right ${t.tipo === 1 ? "text-emerald-700" : "text-rose-700"}`}>
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
  );
}
