import { useEffect, useMemo, useState } from "react";
import toast from "react-hot-toast";
import { useNavigate } from "react-router-dom";
import { listarTransacoes } from "../Services/TransacaoService";
import type { TransacaoResponse, UsuarioResumoResponse } from "../Types/index";
import axios from "axios";
import { BarChart, XAxis, Bar, Legend, YAxis, Tooltip, ResponsiveContainer } from "recharts";

export default function RelatorioFinanceiro() {
  const navigate = useNavigate();
  const [transacoes, setTransacoes] = useState<TransacaoResponse[]>([]);

  const filtrosSalvos = JSON.parse(localStorage.getItem("filtros-dashboard") || "{}");

  const [mesSelecionado, setMesSelecionado] = useState<string>(filtrosSalvos.mes || "todos");

  const [tipoSelecionado, setTipoSelecionado] = useState<number | "todos">(filtrosSalvos.tipo ?? "todos");

  const [loading, setLoading] = useState(true);

  const [animando, setAnimando] = useState(false);

  useEffect(() => {
    async function carregarDados() {
      try {
        setLoading(true);
        const listaTransacoes = await listarTransacoes();
        setTransacoes(listaTransacoes);
      } catch (error: unknown) {
        if (axios.isAxiosError(error)) {
          if (error.response?.status !== 401) {
            toast.error("Erro ao carregar transações");
          }
        } else {
          console.error(error);
        }
      } finally {
        setLoading(false);
      }
    }

    carregarDados();
  }, []);

  const transacoesFiltradas = useMemo(() => {
    let resultados = [...transacoes];

    // Filtro por mês
    if (mesSelecionado !== "todos") {
      resultados = resultados.filter((t) => {
        const data = new Date(t.data);
        const mesAno = `${data.getFullYear()}-${String(data.getMonth() + 1).padStart(2, "0")}`;
        return mesAno === mesSelecionado;
      });
    }

    // Filtro por tipo
    if (tipoSelecionado !== "todos") {
      resultados = resultados.filter((t) => t.tipo === Number(tipoSelecionado));
    }

    return resultados;
  }, [mesSelecionado, tipoSelecionado, transacoes]);

  useEffect(() => {
    setAnimando(true);

    const timeout = setTimeout(() => {
      setAnimando(false);
    }, 200);

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

      if (transacao.tipo === 1) {
        usuario.totalReceitas += transacao.valor;
      } else {
        usuario.totalDespesas += transacao.valor;
      }

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

  const saldoGeralFinal = useMemo(() => totalGeralReceitas - totalGeralDespesas, [totalGeralReceitas, totalGeralDespesas]);

  const ultimasTransacoes = useMemo(() => {
    return [...transacoesFiltradas].sort((a, b) => new Date(b.data).getTime() - new Date(a.data).getTime()).slice(0, 5);
  }, [transacoesFiltradas]);

  const maiorTransacaoId = useMemo(() => {
    if (ultimasTransacoes.length === 0) return null;

    return ultimasTransacoes.reduce((maior, atual) => (Math.abs(atual.valor) > Math.abs(maior.valor) ? atual : maior)).id;
  }, [ultimasTransacoes]);

  function gerarMeses() {
    const meses = [];
    const hoje = new Date();

    for (let i = 0; i < 12; i++) {
      const data = new Date(hoje.getFullYear(), hoje.getMonth() - i, 1);
      const valor = `${data.getFullYear()}-${String(data.getMonth() + 1).padStart(2, "0")}`;
      const label = data.toLocaleDateString("pt-BR", {
        month: "long",
        year: "numeric",
      });

      meses.push({ valor, label });
    }

    return meses;
  }

  const dadosPorMes = useMemo(() => {
    const mapa = new Map<string, { receita: number; despesa: number }>();

    transacoesFiltradas.forEach((t) => {
      const data = new Date(t.data);
      const chave = `${String(data.getMonth() + 1).padStart(2, "0")}/${String(data.getFullYear()).slice(2)}`;

      if (!mapa.has(chave)) {
        mapa.set(chave, { receita: 0, despesa: 0 });
      }

      const item = mapa.get(chave)!;

      if (t.tipo === 1) {
        item.receita += t.valor;
      } else {
        item.despesa += t.valor;
      }
    });

    return Array.from(mapa.entries()).map(([mes, valores]) => ({
      mes,
      receita: valores.receita,
      despesa: valores.despesa,
    }));
  }, [transacoesFiltradas]);

  function formatarValor(valor: number) {
    return valor.toLocaleString("pt-BR", {
      style: "currency",
      currency: "BRL",
    });
  }

  return (
    <div>
      <div className="flex-row md:flex items-center justify-between ">
        <h1 className="text-4xl font-bold mb-4 text-[#2F4F4F]">Relatório Financeiro</h1>
        <div className="flex-row md:flex items-center gap-4">
          {/* FILTROS */}
          <div>
            <h1 className="text-2xl font-bold mb-4 text-[#2F4F4F]">Filtros</h1>
          </div>
          <div className="bg-white  border border-[#C8D6D1] rounded-lg shadow-sm px-4 py-3 mb-4 max-w-xl">
            <div className="flex flex-wrap items-end gap-4">
              {/* MÊS */}
              <div className="flex flex-col">
                <label htmlFor="select-mês" className="text-xs text-[#5A7067] mb-1">
                  Mês
                </label>
                <select
                  id="select-mês"
                  value={mesSelecionado}
                  onChange={(e) => setMesSelecionado(e.target.value)}
                  className="border px-2 py-1.5 rounded border-[#9DB4AB] bg-white focus:outline-none focus:border-[#7A9D8F]"
                >
                  <option value="todos">Todos</option>
                  {gerarMeses().map((mes) => (
                    <option key={mes.valor} value={mes.valor}>
                      {mes.label.charAt(0).toUpperCase() + mes.label.slice(1)}
                    </option>
                  ))}
                </select>
              </div>

              {/* TIPO */}
              <div className="flex flex-col">
                <label htmlFor="select-tipo" className="text-xs text-[#5A7067] mb-1">
                  Tipo
                </label>
                <select
                  id="select-tipo"
                  value={tipoSelecionado}
                  onChange={(e) => setTipoSelecionado(e.target.value === "todos" ? "todos" : Number(e.target.value))}
                  className="border px-2 py-1.5 rounded border-[#9DB4AB] bg-white focus:outline-none focus:border-[#7A9D8F]"
                >
                  <option value="todos">Todos</option>
                  <option value={1}>Receita</option>
                  <option value={2}>Despesa</option>
                </select>
              </div>
              <button
                onClick={limparFiltros}
                disabled={!filtrosAtivos}
                className={` mb-2 transition ${filtrosAtivos ? "text-[#2F4F4F] hover:underline" : "text-gray-400 cursor-not-allowed"}`}
              >
                Limpar
              </button>
            </div>
          </div>
        </div>
      </div>
      {/* RESUMO GERAL */}
      <div className="bg-[#F5F7F6] rounded-lg p-4 mb-4 border border-black/5 shadow-[0_4px_14px_rgba(0,0,0,0.08)]">
        <h2 className="text-3xl font-semibold mb-4 text-[#2F4F4F]">Resumo Geral</h2>

        <hr className="mb-4 border-[#9DB4AB]" />

        <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
          <div className="text-center p-6 bg-white rounded-lg border border-[#C8D6D1]">
            <p className="text-[#5A7067] mb-2">Receitas</p>
            <p className="text-green-600 font-bold text-2xl">{formatarValor(totalGeralReceitas)}</p>
          </div>

          <div className="text-center p-6 bg-white rounded-lg border border-[#C8D6D1]">
            <p className="text-[#5A7067] mb-2">Despesas</p>
            <p className="text-red-600 font-bold text-2xl">{formatarValor(totalGeralDespesas)}</p>
          </div>

          <div className="text-center p-6 bg-white rounded-lg border-2 border-[#C8D6D1]">
            <p className="text-[#2F4F4F] mb-2 font-semibold">Saldo Final</p>
            <p className={`font-bold text-4xl ${saldoGeralFinal >= 0 ? "text-green-700" : "text-red-700"}`}>{formatarValor(saldoGeralFinal)}</p>
          </div>
        </div>
      </div>
      {/* GRÁFICO EVOLUÇÃO */}
      <div className="bg-[#F5F7F6] rounded-lg p-4 mb-4 border border-black/5 shadow">
        <h2 className="text-2xl font-semibold mb-4 text-[#2F4F4F]">Evolução do Patrimônio</h2>

        <div className="w-full h-[250px]">
          <ResponsiveContainer>
            <BarChart data={dadosPorMes} barGap={8}>
              <XAxis dataKey="mes" />
              <YAxis tickFormatter={(valor) => `R$ ${valor}`} />
              <Tooltip
                contentStyle={{
                  backgroundColor: "#fff",
                  borderRadius: "8px",
                  border: "1px solid #ddd",
                }}
                formatter={(value) => (typeof value === "number" ? formatarValor(value) : value)}
                labelFormatter={(label) => `Mês: ${label}`}
              />
              <Legend />
              {/* RECEITAS */}
              <Bar dataKey="receita" name="Receitas" fill="#16a34a" radius={[4, 4, 0, 0]} />

              {/* DESPESAS */}
              <Bar dataKey="despesa" name="Despesas" fill="#dc2626" radius={[4, 4, 0, 0]} />
            </BarChart>
          </ResponsiveContainer>
        </div>
      </div>

      {/* ÚLTIMAS TRANSAÇÕES */}
      <div className="bg-[#F5F7F6] rounded-lg p-3 mb-4 border border-black/5 shadow-[0_4px_14px_rgba(0,0,0,0.08)]">
        <div className="flex justify-between items-center mb-4">
          <h2 className="text-2xl font-semibold text-[#2F4F4F]">Últimas Transações</h2>

          <button onClick={() => navigate("/transacao")} className="text-sm text-[#2F4F4F] hover:underline">
            Ver todas
          </button>
        </div>

        <hr className="mb-4 border-[#9DB4AB]" />

        {loading ? (
          <div className="space-y-3">
            {[...Array(5)].map((_, i) => (
              <div key={i} className="animate-pulse bg-white p-4 rounded border border-[#C8D6D1]">
                <div className="h-4 bg-gray-200 rounded w-1/2 mb-2"></div>
                <div className="h-3 bg-gray-200 rounded w-1/3"></div>
              </div>
            ))}
          </div>
        ) : ultimasTransacoes.length === 0 ? (
          <p className="text-[#5A7067]">Nenhuma transação recente</p>
        ) : (
          <div className={`space-y-3 ${animando ? "opacity-0 translate-y-2 transition-all duration-200" : "opacity-100"}`}>
            {ultimasTransacoes.map((t) => (
              <div
                key={t.id}
                className={`flex justify-between items-center p-4 rounded border transition ${
                  t.id === maiorTransacaoId ? "bg-[#EEF5F2] border-[#7A9D8F] shadow-md" : "bg-white border-[#C8D6D1]"
                }`}
              >
                <div>
                  <p className="font-semibold text-[#2F4F4F] flex items-center gap-2">
                    <span className="text-lg">{t.tipo === 1 ? "💰" : "💸"}</span>
                    {t.descricao}
                  </p>
                  <p className="text-sm text-[#5A7067]">{new Date(t.data).toLocaleDateString("pt-BR")}</p>
                </div>

                <p className={`font-bold ${t.tipo === 1 ? "text-green-600" : "text-red-600"}`}>
                  {t.tipo === 1 ? "+" : "-"} {formatarValor(t.valor)}
                </p>
              </div>
            ))}
          </div>
        )}
      </div>

      {!loading && resumoPorUsuario.length === 0 && (
        <p className="text-center text-[#2F4F4F] mt-8">Nenhuma transação encontrada para os filtros selecionados</p>
      )}
    </div>
  );
}
