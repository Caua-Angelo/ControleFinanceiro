import { useEffect, useMemo, useState } from "react";
import toast from "react-hot-toast";
import { listarTransacoes } from "../Services/TransacaoService";
import type { TransacaoResponse, UsuarioResumoResponse } from "../Types/index";
import axios from "axios";

export default function RelatorioFinanceiro() {
  const [transacoes, setTransacoes] = useState<TransacaoResponse[]>([]);

  const [mesSelecionado, setMesSelecionado] = useState<string>("todos");
  const [tipoSelecionado, setTipoSelecionado] = useState<number | "todos">("todos");

  const [loading, setLoading] = useState(true);

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

  const totalGeralReceitas = useMemo(() => resumoPorUsuario.reduce((acc, curr) => acc + curr.totalReceitas, 0), [resumoPorUsuario]);

  const totalGeralDespesas = useMemo(() => resumoPorUsuario.reduce((acc, curr) => acc + curr.totalDespesas, 0), [resumoPorUsuario]);

  const saldoGeralFinal = useMemo(() => totalGeralReceitas - totalGeralDespesas, [totalGeralReceitas, totalGeralDespesas]);

  const ultimasTransacoes = useMemo(() => {
    return [...transacoesFiltradas].sort((a, b) => new Date(b.data).getTime() - new Date(a.data).getTime()).slice(0, 5);
  }, [transacoesFiltradas]);

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

  function formatarValor(valor: number) {
    return valor.toLocaleString("pt-BR", {
      style: "currency",
      currency: "BRL",
    });
  }

  if (loading) {
    return (
      <div className="flex justify-center items-center h-full">
        <p className="text-[#2F4F4F] text-xl">Carregando...</p>
      </div>
    );
  }
  return (
    <div>
      <h1 className="text-4xl font-bold mb-6 text-[#2F4F4F]">Relatório Financeiro</h1>
      {/* RESUMO GERAL */}
      <div className="bg-[#F5F7F6] rounded-lg p-6 mb-6 border border-black/5 shadow-[0_4px_14px_rgba(0,0,0,0.08)]">
        <h2 className="text-3xl font-semibold mb-4 text-[#2F4F4F]">Resumo Geral</h2>

        <hr className="mb-6 border-[#9DB4AB]" />

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

      {/* FILTROS */}
      <div className="bg-[#F5F7F6] rounded-lg p-6 mb-6 border border-black/5 shadow-[0_4px_14px_rgba(0,0,0,0.08)]">
        <h2 className="text-2xl font-semibold mb-4 text-[#2F4F4F]">Filtros</h2>
        <hr className="mb-4 border-[#9DB4AB]" />

        <div className="grid grid-cols-2 gap-4">
          {/* MÊS */}
          <div>
            <label htmlFor="mes-selecionado" className="text-xl text-[#2F4F4F] mb-2 block">
              Mês
            </label>
            <select
              id="mes-selecionado"
              value={mesSelecionado}
              onChange={(e) => setMesSelecionado(e.target.value)}
              className="w-full border p-2 rounded border-[#9DB4AB] bg-white focus:outline-none focus:border-[#7A9D8F]"
            >
              <option value="todos">Todos os meses</option>
              {gerarMeses().map((mes) => (
                <option key={mes.valor} value={mes.valor}>
                  {mes.label.charAt(0).toUpperCase() + mes.label.slice(1)}
                </option>
              ))}
            </select>
          </div>

          {/* TIPO */}
          <div>
            <label htmlFor="tipo-selecionado" className="text-xl text-[#2F4F4F] mb-2 block">
              Tipo
            </label>
            <select
              id="tipo-selecionado"
              value={tipoSelecionado}
              onChange={(e) => setTipoSelecionado(e.target.value === "todos" ? "todos" : Number(e.target.value))}
              className="w-full border p-2 rounded border-[#9DB4AB] bg-white focus:outline-none focus:border-[#7A9D8F]"
            >
              <option value="todos">Todos</option>
              <option value={1}>Receita</option>
              <option value={2}>Despesa</option>
            </select>
          </div>
        </div>
      </div>
      {/* ÚLTIMAS TRANSAÇÕES */}
      <div className="bg-[#F5F7F6] rounded-lg p-6 mb-6 border border-black/5 shadow-[0_4px_14px_rgba(0,0,0,0.08)]">
        <h2 className="text-2xl font-semibold mb-4 text-[#2F4F4F]">Últimas Transações</h2>

        <hr className="mb-4 border-[#9DB4AB]" />

        {ultimasTransacoes.length === 0 ? (
          <p className="text-[#5A7067]">Nenhuma transação recente</p>
        ) : (
          <div className="space-y-3">
            {ultimasTransacoes.map((t) => (
              <div key={t.id} className="flex justify-between items-center bg-white p-4 rounded border border-[#C8D6D1]">
                <div>
                  <p className="font-semibold text-[#2F4F4F]">{t.descricao}</p>
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
