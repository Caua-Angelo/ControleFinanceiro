import { useEffect, useState } from "react";
import { listarTransacoes } from "../Services/TransacaoService";
import { consultarUsuario } from "../Services/UsuarioService";
import type { TransacaoResponse } from "../Types/TransacaoResponse";
import type { UsuarioResponse } from "../Types/UsuarioResponse";

interface UsuarioResumo {
  usuarioId: number;
  usuarioNome: string;
  totalReceitas: number;
  totalDespesas: number;
  saldoFinal: number;
}
export default function RelatorioFinanceiro() {
  const [transacoes, setTransacoes] = useState<TransacaoResponse[]>([]);
  const [usuarios, setUsuarios] = useState<UsuarioResponse[]>([]);
  const [transacoesFiltradas, setTransacoesFiltradas] = useState<TransacaoResponse[]>([]);
  const [resumoPorUsuario, setResumoPorUsuario] = useState<UsuarioResumo[]>([]);
  const [totalGeralReceitas, setTotalGeralReceitas] = useState(0);
  const [totalGeralDespesas, setTotalGeralDespesas] = useState(0);
  const [saldoGeralFinal, setSaldoGeralFinal] = useState(0);

  // Filtros
  const [usuarioSelecionado, setUsuarioSelecionado] = useState<number | "todos">("todos");
  const [mesSelecionado, setMesSelecionado] = useState<string>("todos");
  const [tipoSelecionado, setTipoSelecionado] = useState<number | "todos">("todos");

  async function ListarTransacoes() {
    try {
      const transacoesData = await listarTransacoes();
      setTransacoes(transacoesData);
      setTransacoesFiltradas(transacoesData); // Inicialmente mostra todas
    } catch (error) {
      console.error(error);
      alert("Erro ao carregar transações");
    }
  }

  async function ListarUsuarios() {
    try {
      const usuariosData = await consultarUsuario();
      setUsuarios(usuariosData);
    } catch (error) {
      console.error(error);
      alert("Erro ao carregar usuários");
    }
  }

  // Aplicar filtros


  function calcularResumos(transacoesData: TransacaoResponse[]) {
    // Agrupar por usuário
    const usuariosMap = new Map<number, UsuarioResumo>();

    transacoesData.forEach((transacao) => {
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
      } else if (transacao.tipo === 2) {
        usuario.totalDespesas += transacao.valor;
      }

      usuario.saldoFinal = usuario.totalReceitas - usuario.totalDespesas;
    });

    const resumos = Array.from(usuariosMap.values());
    setResumoPorUsuario(resumos);

    // Calcular totais gerais
    const totalReceitas = resumos.reduce(
      (acc, curr) => acc + curr.totalReceitas,
      0,
    );
    const totalDespesas = resumos.reduce(
      (acc, curr) => acc + curr.totalDespesas,
      0,
    );
    const saldoFinal = totalReceitas - totalDespesas;

    setTotalGeralReceitas(totalReceitas);
    setTotalGeralDespesas(totalDespesas);
    setSaldoGeralFinal(saldoFinal);
  }
  // Gerar lista de meses
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

  function formatarValor(valor: number): string {
    return valor.toLocaleString("pt-BR", {
      style: "currency",
      currency: "BRL",
    });
  }
  useEffect(() => {
    let resultados = [...transacoes];

    // Filtrar por usuário
    if (usuarioSelecionado !== "todos") {
      resultados = resultados.filter((t) => t.usuarioId === usuarioSelecionado);
    }
    // Filtrar por mês
    if (mesSelecionado !== "todos") {
      resultados = resultados.filter((t) => {
        const dataTransacao = new Date(t.data);
        const mesAno = `${dataTransacao.getFullYear()}-${String(dataTransacao.getMonth() + 1).padStart(2, "0")}`;
        return mesAno === mesSelecionado;
      });
    }
    // Filtrar por categoria
    if (tipoSelecionado !== "todos") {
      resultados = resultados.filter(
        (t) => t.tipo === Number(tipoSelecionado)
      );
    }

    setTransacoesFiltradas(resultados);
    calcularResumos(resultados);
  }, [usuarioSelecionado, mesSelecionado, tipoSelecionado, transacoes]);

  useEffect(() => {
    ListarTransacoes();
    ListarUsuarios();
  }, []);

  return (
    <div className="p-6">
      <h1 className="text-4xl font-bold mb-6 text-[#2F4F4F]">
        Relatório Financeiro
      </h1>

      {/* Filtros */}
      <div className="bg-[#F5F7F6] rounded-lg  p-6 mb-6 border border-black/5 shadow-[0_4px_14px_rgba(0,0,0,0.08)]">
        <h2 className="text-2xl font-semibold mb-4 text-[#2F4F4F]">Filtros</h2>
        <hr className="mb-4 border-[#9DB4AB]" />

        <div className="grid grid-cols-2 gap-4">
          {/* Filtro de Usuário */}
          <div>
            <label
              htmlFor="select-usuario"
              className="block mb-2 text-[#2F4F4F] font-medium"
            >
              Usuário
            </label>
            <select
              id="select-usuario"
              value={usuarioSelecionado}
              onChange={(e) =>
                setUsuarioSelecionado(
                  e.target.value === "todos" ? "todos" : Number(e.target.value),
                )
              }
              className="w-full border p-2 rounded border-[#9DB4AB] bg-white focus:outline-none focus:border-[#7A9D8F]"
            >
              <option value="todos">Todos os usuários</option>
              {usuarios.map((usuario) => (
                <option key={usuario.id} value={usuario.id}>
                  {usuario.nome}
                </option>
              ))}
            </select>
          </div>

          {/* Filtro de Mês */}
          <div>
            <label
              htmlFor="select-mes"
              className="block mb-2 text-[#2F4F4F] font-medium"
            >
              Mês
            </label>
            <select
              id="select-mes"
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
          {/* Filtro de Tipo */}
          <div>
            <label
              htmlFor="select-TipoSelecionado"
              className="block mb-2 text-[#2F4F4F] font-medium"
            >
              Categoria
            </label>
            <select
              id="select-TipoSelecionado"
              value={tipoSelecionado}
              onChange={(e) =>
                setTipoSelecionado(
                  e.target.value === "todos" ? "todos" : Number(e.target.value)
                )
              }
              className="w-full border p-2 rounded border-[#9DB4AB] bg-white focus:outline-none focus:border-[#7A9D8F]"
            >
              <option value="todos">Todos</option>
              <option value={1}>Receita</option>
              <option value={2}>Despesa</option>
            </select>
          </div>

        </div>

      </div>

      {/* Cards individuais por usuário */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6 mb-6">
        {resumoPorUsuario.map((usuario) => (
          <div
            key={usuario.usuarioId}
            className="bg-[#F5F7F6] rounded-lg p-6 border border-black/5 shadow-[0_4px_14px_rgba(0,0,0,0.08)] transition hover:shadow-[0_8px_20px_rgba(0,0,0,0.12)]"
          >
            <h2 className="text-2xl font-semibold mb-4 text-[#2F4F4F]">
              {usuario.usuarioNome}
            </h2>
            <hr className="mb-4 border-[#9DB4AB]" />

            <div className="space-y-3">
              <div className="flex justify-between items-center">
                <span className="text-[#5A7067]">Total de Receitas:</span>
                <span className="text-green-600 font-semibold text-lg">
                  {formatarValor(usuario.totalReceitas)}
                </span>
              </div>

              <div className="flex justify-between items-center">
                <span className="text-[#5A7067]">Total de Despesas:</span>
                <span className="text-red-600 font-semibold text-lg">
                  {formatarValor(usuario.totalDespesas)}
                </span>
              </div>

              <hr className="border-[#C8D6D1]" />

              <div className="flex justify-between items-center">
                <span className="text-[#2F4F4F] font-semibold">
                  Saldo Final:
                </span>
                <span
                  className={`font-bold text-xl ${usuario.saldoFinal >= 0 ? "text-green-700" : "text-red-700"
                    }`}
                >
                  {formatarValor(usuario.saldoFinal)}
                </span>
              </div>
            </div>
          </div>
        ))}
      </div>

      {/* Card de Resumo Geral */}
      <div className="bg-[#F5F7F6] rounded-lg p-8 border border-black/5 shadow-[0_4px_14px_rgba(0,0,0,0.08)] transition hover:shadow-[0_8px_20px_rgba(0,0,0,0.12)]">
        <h2 className="text-3xl font-semibold mb-4 text-[#2F4F4F]">
          Resumo Geral
        </h2>
        <hr className="mb-6 border-[#9DB4AB]" />

        <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
          <div className="text-center p-6 bg-white rounded-lg border border-[#C8D6D1]">
            <p className="text-[#5A7067] mb-2 text-lg">Total de Receitas</p>
            <p className="text-green-600 font-bold text-3xl">
              {formatarValor(totalGeralReceitas)}
            </p>
          </div>

          <div className="text-center p-6 bg-white rounded-lg border border-[#C8D6D1]">
            <p className="text-[#5A7067] mb-2 text-lg">Total de Despesas</p>
            <p className="text-red-600 font-bold text-3xl">
              {formatarValor(totalGeralDespesas)}
            </p>
          </div>

          <div className="text-center p-6 bg-white rounded-lg border border-[#C8D6D1]">
            <p className="text-[#2F4F4F] mb-2 text-lg font-semibold">
              Saldo Geral Final
            </p>
            <p
              className={`font-bold text-4xl ${saldoGeralFinal >= 0 ? "text-green-700" : "text-red-700"
                }`}
            >
              {formatarValor(saldoGeralFinal)}
            </p>
          </div>
        </div>
      </div>

      {resumoPorUsuario.length === 0 && (
        <p className="text-center text-[#2F4F4F] mt-8">
          Nenhuma transação encontrada para gerar o relatório
        </p>
      )}
    </div>
  );
}
