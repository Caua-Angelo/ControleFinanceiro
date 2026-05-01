import { useEffect, useMemo, useState } from "react";
import { listarTransacoes } from "../Services/TransacaoService";
import type { TransacaoResponse, UsuarioResumoResponse } from "../Types/index";
import axios from "axios";

export default function RelatorioFinanceiro() {
  const [transacoes, setTransacoes] = useState<TransacaoResponse[]>([]);

  const [mesSelecionado, setMesSelecionado] = useState<string>("todos");
  const [tipoSelecionado, setTipoSelecionado] = useState<number | "todos">("todos");

  useEffect(() => {
    async function carregarDados() {
      try {
        const listaTransacoes = await listarTransacoes();
        setTransacoes(listaTransacoes);
      } catch (error: unknown) {
        if (axios.isAxiosError(error)) {
          if (error.response?.status !== 401) {
            alert("Erro ao carregar transações");
          }
        } else {
          console.error(error);
        }
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

  return (
    <div>
      <h1 className="text-4xl font-bold mb-6 text-[#2F4F4F]">Relatório Financeiro</h1>

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
              Categoria
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

      {/* CARDS POR USUÁRIO */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6 mb-6">
        {resumoPorUsuario.map((usuario) => (
          <div
            key={usuario.usuarioId}
            className="bg-[#F5F7F6] rounded-lg p-6 border border-black/5 shadow-[0_4px_14px_rgba(0,0,0,0.08)] transition hover:shadow-[0_8px_20px_rgba(0,0,0,0.12)]"
          >
            <h2 className="text-2xl font-semibold mb-4 text-[#2F4F4F]">{usuario.usuarioNome}</h2>

            <hr className="mb-4 border-[#9DB4AB]" />

            <div className="space-y-3">
              <div className="flex justify-between">
                <span className="text-[#5A7067]">Receitas:</span>
                <span className="text-green-600 font-semibold">{formatarValor(usuario.totalReceitas)}</span>
              </div>

              <div className="flex justify-between">
                <span className="text-[#5A7067]">Despesas:</span>
                <span className="text-red-600 font-semibold">{formatarValor(usuario.totalDespesas)}</span>
              </div>

              <hr className="border-[#C8D6D1]" />

              <div className="flex justify-between">
                <span className="font-semibold text-[#2F4F4F]">Saldo Final:</span>
                <span className={`font-bold ${usuario.saldoFinal >= 0 ? "text-green-700" : "text-red-700"}`}>
                  {formatarValor(usuario.saldoFinal)}
                </span>
              </div>
            </div>
          </div>
        ))}
      </div>

      {/* RESUMO GERAL */}
      <div className="bg-[#F5F7F6] rounded-lg p-8 border border-black/5 shadow-[0_4px_14px_rgba(0,0,0,0.08)]">
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

          <div className="text-center p-6 bg-white rounded-lg border border-[#C8D6D1]">
            <p className="text-[#2F4F4F] mb-2 font-semibold">Saldo Final</p>
            <p className={`font-bold text-3xl ${saldoGeralFinal >= 0 ? "text-green-700" : "text-red-700"}`}>{formatarValor(saldoGeralFinal)}</p>
          </div>
        </div>
      </div>

      {resumoPorUsuario.length === 0 && <p className="text-center text-[#2F4F4F] mt-8">Nenhuma transação encontrada</p>}
    </div>
  );
}
