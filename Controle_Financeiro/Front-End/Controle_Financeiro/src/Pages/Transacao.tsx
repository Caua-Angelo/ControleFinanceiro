import { useEffect, useState } from "react";
import { criarTransacao, listarTransacoes, deletarTransacao, alterarTransacao, type TransacaoRequest } from "../Services/TransacaoService";
import { consultarCategoria } from "../Services/CategoriaService";
import { getUsuarioLogado } from "../Services/UsuarioService";
import type { CategoriaResponse } from "../Types/CategoriaResponse";
import type { TransacaoResponse } from "../Types/TransacaoResponse";
import { Button } from "../Components/Button";

export default function Transacao() {
  const [descricao, setDescricao] = useState("");
  const [valor, setValor] = useState<string>("");
  const [tipo, setTipo] = useState<number | "">("");
  const [categoriaId, setCategoriaId] = useState<number | "">("");
  const [data, setData] = useState<string>("");

  const [transacoes, setTransacoes] = useState<TransacaoResponse[]>([]);
  const [categorias, setCategorias] = useState<CategoriaResponse[]>([]);
  const [usuario, setUsuario] = useState<{ idade: number } | null>(null);

  const [showModal, setShowModal] = useState(false);
  const [transacaoSelecionada, setTransacaoSelecionada] = useState<TransacaoResponse | null>(null);

  const [descricaoEdit, setDescricaoEdit] = useState("");
  const [valorEdit, setValorEdit] = useState<string>("");
  const [tipoEdit, setTipoEdit] = useState<number | "">("");
  const [categoriaIdEdit, setCategoriaIdEdit] = useState<number | "">("");
  const [dataEdit, setDataEdit] = useState<string>("");

  const [loadingCriar, setLoadingCriar] = useState(false);
  const [loadingEditar, setLoadingEditar] = useState(false);
  const [deletingId, setDeletingId] = useState<number | null>(null);

  const isMenorDeIdade = usuario ? usuario.idade < 18 : false;

  useEffect(() => {
    async function carregarDados() {
      try {
        const [transacoesData, categoriasData, usuarioData] = await Promise.all([listarTransacoes(), consultarCategoria(), getUsuarioLogado()]);

        setTransacoes(transacoesData);
        setCategorias(categoriasData);
        setUsuario(usuarioData);
      } catch (error) {
        console.error(error);
        alert("Erro ao carregar dados");
      }
    }

    carregarDados();
  }, []);

  const categoriasFiltradas = categorias.filter((categoria) => {
    if (tipo === "") return false;

    if (tipo === 1) {
      return categoria.finalidade === 2 || categoria.finalidade === 3;
    } else {
      return categoria.finalidade === 1 || categoria.finalidade === 3;
    }
  });

  async function CriarTransacao() {
    try {
      if (!descricao || valor === "" || tipo === "" || categoriaId === "" || !data) {
        alert("Preencha todos os campos");
        return;
      }

      const valorNumerico = Number(valor.replace(",", "."));

      if (valorNumerico <= 0) {
        alert("Valor inválido");
        return;
      }
      setLoadingCriar(true);

      const payload: TransacaoRequest = {
        descricao,
        valor: valorNumerico,
        tipo: Number(tipo),
        categoriaId: Number(categoriaId),
        data: new Date(data).toISOString(),
        usuarioId: 0, // TEMPORÁRIO
      };

      await criarTransacao(payload);
      await listarTodasTransacoes();

      setDescricao("");
      setValor("");
      setTipo("");
      setCategoriaId("");
      setData("");

      alert("Transação criada com sucesso ✅");
    } catch (error) {
      console.error(error);
      alert("Erro ao criar transação");
    } finally {
      setLoadingCriar(false);
    }
  }

  async function listarTodasTransacoes() {
    const data = await listarTransacoes();
    setTransacoes(data);
  }

  async function DeletarTransacao(id: number) {
    if (!window.confirm("Tem certeza que deseja excluir?")) return;

    try {
      setDeletingId(id);

      await deletarTransacao(id);
      await listarTodasTransacoes();

      alert("Transação excluída com sucesso ✅");
    } catch (error) {
      console.error(error);
      alert("Erro ao excluir transação ❌");
    } finally {
      setDeletingId(null);
    }
  }

  function abrirModalEditar(transacao: TransacaoResponse) {
    setTransacaoSelecionada(transacao);
    setDescricaoEdit(transacao.descricao);
    setValorEdit(transacao.valor.toFixed(2).replace(".", ","));
    setTipoEdit(transacao.tipo);
    setCategoriaIdEdit(transacao.categoriaId);
    setDataEdit(transacao.data.split("T")[0]);
    setShowModal(true);
  }

  async function editarTransacao() {
    if (!transacaoSelecionada) return;

    try {
      setLoadingEditar(true);

      const valorNumerico = Number(valorEdit.replace(",", "."));

      await alterarTransacao(transacaoSelecionada.id, {
        descricao: descricaoEdit,
        valor: valorNumerico,
        tipo: Number(tipoEdit),
        categoriaId: Number(categoriaIdEdit),
        data: dataEdit,
        usuarioId: 0,
      });

      await listarTodasTransacoes();

      alert("Transação atualizada com sucesso ✅");

      setShowModal(false);
    } catch (error) {
      console.error(error);
      alert("Erro ao atualizar transação ❌");
    } finally {
      setLoadingEditar(false);
    }
  }

  function formatarValor(valor: number, tipo: number) {
    const sinal = tipo === 1 ? "+ " : "- ";
    return (
      sinal +
      valor.toLocaleString("pt-BR", {
        style: "currency",
        currency: "BRL",
      })
    );
  }

  function formatarData(dataISO: string) {
    const [ano, mes, dia] = dataISO.split("T")[0].split("-");
    return `${dia}/${mes}/${ano}`;
  }

  return (
    <div>
      {/* FORM */}
      <div className=" bg-[#F5F7F6] rounded-lg p-6 max-w-5xl mx-auto w-full shadow-[0_4px_14px_rgba(0,0,0,0.08)] border border-black/5 transition hover:shadow-[0_8px_20px_rgba(0,0,0,0.12)]">
        <h2 className="text-3xl font-semibold mb-6 text-[#2F4F4F]">Adicionar Nova Transação</h2>

        <div className="grid grid-cols-2 gap-4">
          <div>
            <label htmlFor="descricao" className="text-xl text-[#2F4F4F] mb-2">
              Descrição
            </label>
            <input
              id="descricao"
              className="w-full rounded border border-[#9DB4AB] bg-white p-2 focus:outline-none focus:border-[#7A9D8F]"
              value={descricao}
              onChange={(e) => setDescricao(e.target.value)}
            />
          </div>

          <div>
            <label htmlFor="valor" className="text-xl text-[#2F4F4F] mb-2">
              Valor
            </label>
            <input
              id="valor"
              className="w-full rounded border border-[#9DB4AB] bg-white p-2 focus:outline-none focus:border-[#7A9D8F]"
              value={valor}
              onChange={(e) => setValor(e.target.value)}
            />
          </div>

          <div>
            <label htmlFor="data" className="text-xl text-[#2F4F4F] mb-2">
              Data
            </label>
            <input
              id="data"
              type="date"
              className="w-full rounded border border-[#9DB4AB] bg-white p-2 focus:outline-none focus:border-[#7A9D8F]"
              value={data}
              onChange={(e) => setData(e.target.value)}
            />
          </div>

          <div>
            <label htmlFor="tipo" className="text-xl text-[#2F4F4F] mb-2">
              Tipo
            </label>
            <select
              id="tipo"
              className="w-full rounded border border-[#9DB4AB] bg-white p-2 focus:outline-none focus:border-[#7A9D8F]"
              value={tipo}
              onChange={(e) => setTipo(Number(e.target.value))}
            >
              <option value="">Selecione</option>
              {!isMenorDeIdade && <option value={1}>Receita</option>}
              <option value={2}>Despesa</option>
            </select>
          </div>

          <div className="col-span-2">
            <label htmlFor="categoria" className="text-xl text-[#2F4F4F] mb-2">
              Categoria
            </label>
            <select
              id="categoria"
              className="w-full rounded border border-[#9DB4AB] bg-white p-2 focus:outline-none focus:border-[#7A9D8F]"
              value={categoriaId}
              onChange={(e) => setCategoriaId(Number(e.target.value))}
            >
              <option value="">Selecione</option>
              {categoriasFiltradas.map((c) => (
                <option key={c.id} value={c.id}>
                  {c.descricao}
                </option>
              ))}
            </select>
          </div>

          <div className="col-span-2">
            <Button onClick={CriarTransacao} label={loadingCriar ? "Salvando..." : "Criar Transação"} variant="primary" disabled={loadingCriar} />
          </div>
        </div>
      </div>

      {/* LISTA */}
      <div className="mt-6 bg-[#F5F7F6] p-6 rounded-xl max-w-5xl mx-auto w-full shadow-[0_4px_14px_rgba(0,0,0,0.08)] border border-black/5 transition hover:shadow-[0_8px_20px_rgba(0,0,0,0.12)]">
        <h2 className="text-3xl mb-4 text-[#2F4F4F]">Lista de Transações</h2>

        {/* HEADER */}
        <div className="grid grid-cols-[2fr_1fr_1fr_1fr_180px] font-semibold text-[#2F4F4F] mb-2 px-4">
          <div>Descrição</div>
          <div>Valor</div>
          <div>Data</div>
          <div>Tipo</div>
          <div className="text-left pl-14">Ações</div>
        </div>

        {/* LINHAS */}
        {transacoes.map((t, index) => (
          <div
            key={t.id}
            className={`
        grid grid-cols-[2fr_1fr_1fr_1fr_180px] items-center px-4 py-3 mb-2 rounded border border-[#C8D6D1]
        ${index % 2 === 0 ? "bg-white" : "bg-[#F5F7F6]"}
        hover:bg-[#E8EFED]
      `}
          >
            {/* DESCRIÇÃO */}
            <div className="font-medium text-[#2F4F4F]">{t.descricao}</div>
            <div>{formatarValor(t.valor, t.tipo)}</div>
            <div>{formatarData(t.data)}</div>
            <div>{t.tipo === 1 ? "Receita" : "Despesa"}</div>
            <div className="flex justify-start gap-2 pl-4">
              <Button onClick={() => abrirModalEditar(t)} label="Editar" variant="edit" disabled={loadingEditar} />
              <Button
                onClick={() => DeletarTransacao(t.id)}
                label={deletingId === t.id ? "Excluindo..." : "Excluir"}
                variant="delete"
                disabled={deletingId === t.id}
              />
            </div>
          </div>
        ))}
      </div>

      {/* MODAL */}
      {showModal && transacaoSelecionada && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
          <div className="bg-white p-6 rounded-lg w-96 border border-[#9DB4AB]">
            <h2 className="text-2xl font-semibold mb-4 text-[#2F4F4F]">Editar Transação</h2>

            <div>
              <label htmlFor="input-descricao" className="block mb-1 text-[#2F4F4F]">
                Descrição
              </label>
              <input
                id="input-descricao"
                className="w-full border border-[#9DB4AB] p-2 rounded focus:outline-none focus:border-[#7A9D8F]"
                value={descricaoEdit}
                onChange={(e) => setDescricaoEdit(e.target.value)}
              />
            </div>

            <div>
              <label htmlFor="input-valor" className="block mb-1 text-[#2F4F4F]">
                Valor
              </label>
              <input
                id="input-valor"
                className="w-full border border-[#9DB4AB] p-2 rounded focus:outline-none focus:border-[#7A9D8F]"
                value={valorEdit}
                onChange={(e) => setValorEdit(e.target.value)}
              />
            </div>

            <div className="mb-4">
              <label htmlFor="input-data" className="block mb-1 text-[#2F4F4F]">
                Data
              </label>
              <input
                id="input-data"
                type="date"
                className="w-full border border-[#9DB4AB] p-2 rounded focus:outline-none focus:border-[#7A9D8F]"
                value={dataEdit}
                onChange={(e) => setDataEdit(e.target.value)}
              />
            </div>

            <div className="flex justify-center gap-2">
              <Button onClick={editarTransacao} label={loadingEditar ? "Salvando..." : "Salvar"} variant="saveModal" disabled={loadingEditar} />
              <Button onClick={() => setShowModal(false)} label="Cancelar" variant="cancelModal"></Button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
