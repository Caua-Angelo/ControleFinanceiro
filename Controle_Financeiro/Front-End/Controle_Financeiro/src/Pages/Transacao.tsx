import { Fragment, useEffect, useState } from "react";
import toast from "react-hot-toast";
import { criarTransacao, listarTransacoes, deletarTransacao, alterarTransacao, type TransacaoRequest } from "../Services/TransacaoService";
import { consultarCategoria } from "../Services/CategoriaService";
import { getUsuarioLogado, type UsuarioLogadoResponse } from "../Services/UsuarioService";
import type { CategoriaResponse } from "../Types/CategoriaResponse";
import type { TransacaoResponse } from "../Types/TransacaoResponse";
import { Button } from "../Components/Button";
import dayjs from "dayjs";

export default function Transacao() {
  const [descricao, setDescricao] = useState("");
  const [valor, setValor] = useState<string>("");
  const [tipo, setTipo] = useState<number | "">("");
  const [categoriaId, setCategoriaId] = useState<number | "">("");
  const [data, setData] = useState<string>("");

  const [transacoes, setTransacoes] = useState<TransacaoResponse[]>([]);
  const [categorias, setCategorias] = useState<CategoriaResponse[]>([]);
  const [usuario, setUsuario] = useState<UsuarioLogadoResponse | null>(null);

  // filtros da lista
  const [filtroDescricao, setFiltroDescricao] = useState<string>("");
  const [filtroTipo, setFiltroTipo] = useState<number | "">("");
  const [filtroCategoriaId, setFiltroCategoriaId] = useState<number | "">("");

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
        toast.error("Erro ao carregar dados");
      }
    }

    carregarDados();
  }, []);

  const categoriasFiltradas = (Array.isArray(categorias) ? categorias : []).filter((categoria) => {
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
        toast.error("Preencha todos os campos");
        return;
      }
      if (!usuario?.id) {
        toast.error("Usuário não encontrado");
        return;
      }

      const valorNumerico = Number(valor.replace(",", "."));
      const dataFormatada = dayjs(data).format("YYYY-MM-DD");
      if (valorNumerico <= 0) {
        toast.error("Valor inválido");
        return;
      }
      setLoadingCriar(true);

      const payload: TransacaoRequest = {
        descricao,
        valor: valorNumerico,
        tipo: Number(tipo),
        categoriaId: Number(categoriaId),
        data: dataFormatada,
        usuarioId: usuario.id,
      };

      await criarTransacao(payload);
      await listarTodasTransacoes();

      setDescricao("");
      setValor("");
      setTipo("");
      setCategoriaId("");
      setData("");

      toast.success("Transação criada com sucesso ✅");
    } catch (error) {
      console.error(error);
      toast.error("Erro ao criar transação");
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

      toast.success("Transação excluída com sucesso ✅");
    } catch (error) {
      console.error(error);
      toast.error("Erro ao excluir transação ❌");
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
    if (!usuario?.id) {
      toast.error("Usuário não encontrado");
      return;
    }
    try {
      setLoadingEditar(true);

      const valorNumerico = Number(valorEdit.replace(",", "."));
      const dataFormatada = dayjs(dataEdit).format("YYYY-MM-DD");

      await alterarTransacao(transacaoSelecionada.id, {
        descricao: descricaoEdit,
        valor: valorNumerico,
        tipo: Number(tipoEdit),
        categoriaId: Number(categoriaIdEdit),
        data: dataFormatada,
        usuarioId: usuario.id,
      });

      await listarTodasTransacoes();

      toast.success("Transação atualizada com sucesso ✅");

      setShowModal(false);
    } catch (error) {
      console.error(error);
      toast.error("Erro ao atualizar transação ❌");
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

  // aplica filtros na lista em memória
  const transacoesFiltradas = (Array.isArray(transacoes) ? transacoes : []).filter((t) => {
    if (filtroDescricao && !t.descricao.toLowerCase().includes(filtroDescricao.toLowerCase())) return false;
    if (filtroTipo !== "" && t.tipo !== Number(filtroTipo)) return false;
    if (filtroCategoriaId !== "" && t.categoriaId !== Number(filtroCategoriaId)) return false;
    return true;
  });

  return (
    <div>
      {/* FORM */}
      <div className=" bg-[#F5F7F6] rounded-lg p-6 max-w-5xl mx-auto w-full shadow-[0_4px_14px_rgba(0,0,0,0.08)] border border-black/5 transition hover:shadow-[0_8px_20px_rgba(0,0,0,0.12)]">
        <h2 className="text-3xl font-semibold mb-6 text-[#2F4F4F]">Adicionar Nova Transação</h2>

        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
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

          <div className="md:col-span-2">
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

          <div className="md:col-span-2">
            <Button onClick={CriarTransacao} label={loadingCriar ? "Salvando..." : "Criar Transação"} variant="primary" disabled={loadingCriar} />
          </div>
        </div>
      </div>

      {/* LISTA DE TRANSAÇÕES */}
      <div className="mt-6 bg-[#F5F7F6] rounded-lg p-6 max-w-5xl mx-auto w-full shadow-[0_4px_14px_rgba(0,0,0,0.08)] border border-black/5">
        <h2 className="text-3xl font-semibold mb-6 text-[#2F4F4F]">Lista de Transações</h2>

        {/* FILTROS */}
        <div className="mb-4 grid grid-cols-1 md:grid-cols-4 gap-4 items-end">
          <div>
            <label className="text-sm text-[#2F4F4F] mb-1 block">Descrição</label>
            <input
              className="w-full rounded border border-[#9DB4AB] bg-white p-2 focus:outline-none focus:border-[#7A9D8F]"
              value={filtroDescricao}
              onChange={(e) => setFiltroDescricao(e.target.value)}
              placeholder="Buscar por descrição"
            />
          </div>

          <div>
            <label className="text-sm text-[#2F4F4F] mb-1 block">Tipo</label>
            <select
              className="w-full rounded border border-[#9DB4AB] bg-white p-2 focus:outline-none focus:border-[#7A9D8F]"
              value={filtroTipo}
              onChange={(e) => setFiltroTipo(e.target.value === "" ? "" : Number(e.target.value))}
            >
              <option value="">Todos</option>
              <option value={1}>Receita</option>
              <option value={2}>Despesa</option>
            </select>
          </div>

          <div>
            <label className="text-sm text-[#2F4F4F] mb-1 block">Categoria</label>
            <select
              className="w-full rounded border border-[#9DB4AB] bg-white p-2 focus:outline-none focus:border-[#7A9D8F]"
              value={filtroCategoriaId}
              onChange={(e) => setFiltroCategoriaId(e.target.value === "" ? "" : Number(e.target.value))}
            >
              <option value="">Todas</option>
              {(Array.isArray(categorias) ? categorias : []).map((c) => (
                <option key={c.id} value={c.id}>
                  {c.descricao}
                </option>
              ))}
            </select>
          </div>

          <div className="flex gap-2">
            <button
              type="button"
              className="px-4 py-2 rounded bg-[#E2E8E6] text-[#2F4F4F] hover:bg-[#D0DFDA]"
              onClick={() => {
                setFiltroDescricao("");
                setFiltroTipo("");
                setFiltroCategoriaId("");
              }}
            >
              Limpar
            </button>
          </div>
        </div>

        {/* CABEÇALHO DESKTOP */}
        <div className="hidden md:grid grid-cols-[2fr_1fr_1fr_1fr_180px] font-semibold text-[#2F4F4F] mb-2 px-4">
          <div>Descrição</div>
          <div>Valor</div>
          <div>Data</div>
          <div>Tipo</div>
          <div className="text-left pl-4">Ações</div>
        </div>

        {/* LINHAS */}
        {transacoesFiltradas.map((t, index) => (
          <Fragment key={t.id}>
            {/* DESKTOP */}
            <div
              className={`
          hidden md:grid
          grid-cols-[2fr_1fr_1fr_1fr_180px]
          items-center
          px-4 py-3
          mb-2
          rounded
          border border-[#C8D6D1]
          ${index % 2 === 0 ? "bg-white" : "bg-[#F5F7F6]"}
          hover:bg-[#E8EFED]
        `}
            >
              {/* DESCRIÇÃO */}
              <div className="font-medium text-[#2F4F4F]">{t.descricao}</div>

              {/* VALOR */}
              <div>{formatarValor(t.valor, t.tipo)}</div>

              {/* DATA */}
              <div>{formatarData(t.data)}</div>

              {/* TIPO */}
              <div>{t.tipo === 1 ? "Receita" : "Despesa"}</div>

              {/* AÇÕES */}
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

            {/* MOBILE */}
            <div
              className={`
          md:hidden
          p-4
          mb-3
          rounded-lg
          border border-[#C8D6D1]
          ${index % 2 === 0 ? "bg-white" : "bg-[#F5F7F6]"}
        `}
            >
              {/* DESCRIÇÃO */}
              <div className="font-semibold text-lg text-[#2F4F4F] mb-4">{t.descricao}</div>

              {/* INFORMAÇÕES */}
              <div className="space-y-2 text-[#2F4F4F]">
                <div className="flex justify-between gap-4">
                  <span className="font-medium">Valor</span>

                  <span>{formatarValor(t.valor, t.tipo)}</span>
                </div>

                <div className="flex justify-between gap-4">
                  <span className="font-medium">Data</span>

                  <span>{formatarData(t.data)}</span>
                </div>

                <div className="flex justify-between gap-4">
                  <span className="font-medium">Tipo</span>

                  <span>{t.tipo === 1 ? "Receita" : "Despesa"}</span>
                </div>
              </div>

              {/* AÇÕES */}
              <div className="flex gap-2 mt-4">
                <Button onClick={() => abrirModalEditar(t)} label="Editar" variant="edit" disabled={loadingEditar} />

                <Button
                  onClick={() => DeletarTransacao(t.id)}
                  label={deletingId === t.id ? "Excluindo..." : "Excluir"}
                  variant="delete"
                  disabled={deletingId === t.id}
                />
              </div>
            </div>
          </Fragment>
        ))}
      </div>
      {showModal && transacaoSelecionada && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4">
          <div className="bg-white p-6 rounded-lg w-full max-w-md border border-[#9DB4AB]">
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

              <Button onClick={() => setShowModal(false)} label="Cancelar" variant="cancelModal" />
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
