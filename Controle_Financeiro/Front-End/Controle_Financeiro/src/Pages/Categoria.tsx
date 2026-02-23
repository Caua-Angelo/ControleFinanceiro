import { useEffect, useState } from "react";
import {
  criarCategoria,
  consultarCategoria,
  deletarCategoria,
  alterarCategoria,
} from "../Services/CategoriaService";
import { Finalidade } from "../Types/Finalidade";
import type { CategoriaResponse } from "../Types/CategoriaResponse";
import { Button } from "../Components/Button";

export default function Categoria() {
  const [descricao, setDescricao] = useState("");
  const [finalidade, setFinalidade] = useState<Finalidade | "">("");
  const [categorias, setCategorias] = useState<CategoriaResponse[]>([]);
  const [paginaAtual, setPaginaAtual] = useState(1);
  const [descricaoEdit, setDescricaoEdit] = useState("");
  const [finalidadeEdit, setFinalidadeEdit] = useState<Finalidade | "">("");
  const [editandoId, setEditandoId] = useState<number | null>(null);

  const [showModal, setShowModal] = useState(false);
  const categoriasPorPagina = 4;
  const totalPaginas = Math.ceil(categorias.length / categoriasPorPagina);
  const indiceInicial = (paginaAtual - 1) * categoriasPorPagina;
  const indiceFinal = indiceInicial + categoriasPorPagina;
  const categoriasPaginadas = categorias.slice(indiceInicial, indiceFinal);

  async function CriarCategoria() {
    try {
      if (!descricao || finalidade === "") {
        alert("Preencha todos os campos");
        return;
      }

      const payload = {
        descricao,
        finalidade: Number(finalidade),
      };

      const categoriaNova = await criarCategoria(payload);
      setCategorias((prev) => [...prev, categoriaNova]);
      alert("Categoria criada com sucesso!");
      setDescricao("");
      setFinalidade("");
    } catch (error) {
      console.error(error);
      alert("Erro ao criar categoria");
    }
  }

  async function DeletarCategoria(id: number) {
    const confirmar = window.confirm("Tem certeza que deseja excluir?");
    if (!confirmar) return;

    try {
      await deletarCategoria(id);
      setCategorias((prev) => prev.filter((c) => c.id !== id));
      alert("Categoria excluída com sucesso!");
    } catch (error) {
      console.error(error);
      alert("Erro ao excluir categoria");
    }
  }

  useEffect(() => {
    const carregarCategorias = async () => {
      try {
        const data = await consultarCategoria();
        setCategorias(data);
      } catch (error) {
        console.error(error);
        alert("Erro ao carregar categorias");
      }
    };

    carregarCategorias();
  }, []);

  async function editarCategoria() {
    if (!editandoId || !descricaoEdit || finalidadeEdit === "") {
      alert("Preencha todos os campos");
      return;
    }

    try {
      const payload = {
        descricao: descricaoEdit,
        finalidade: Number(finalidadeEdit),
      };

      const categoriaAtualizada = await alterarCategoria(editandoId, payload);

      setCategorias((prev) =>
        prev.map((c) => (c.id === editandoId ? categoriaAtualizada : c)),
      );

      setShowModal(false);
      setEditandoId(null);
      setDescricaoEdit("");
      setFinalidadeEdit("");

      alert("Categoria alterada com sucesso!");
    } catch (error) {
      console.error(error);
      alert("Erro ao alterar categoria");
    }
  }

  function abrirModalEditar(categoria: CategoriaResponse) {
    setEditandoId(categoria.id);
    setDescricaoEdit(categoria.descricao);
    setFinalidadeEdit(categoria.finalidade);
    setShowModal(true);
  }

  return (
    <div className="">
      <div className="flex justify-center gap-4 p-6 backdrop-blur-md rounded-lg"></div>

      <div className="flex flex-col">
        {/* Card de Criar Categoria */}
        <div className="mt-6 bg-[#F5F7F6] rounded-lg p-6 w-290 mx-auto shadow-[0_4px_14px_rgba(0,0,0,0.08)] border border-black/5 transition hover:shadow-[0_8px_20px_rgba(0,0,0,0.12)]">
          <h2 className="text-3xl font-semibold mb-4 text-[#2F4F4F]">
            Adicionar Nova Categoria
          </h2>
          <hr className="mb-4 border-[#9DB4AB]"></hr>

          <div className="flex flex-col gap-4">
            <div className="grid grid-cols-2 gap-4">
              {/* Descrição */}
              <div>
                <label
                  htmlFor="input-descricao"
                  className="text-xl text-[#2F4F4F] mb-2"
                >
                  Descrição
                </label>
                <input
                  type="text"
                  id="input-descricao"
                  placeholder="Ex: Aluguel ou Salário"
                  value={descricao}
                  onChange={(e) => setDescricao(e.target.value)}
                  className="w-full rounded border border-[#9DB4AB] bg-white p-2 focus:outline-none focus:border-[#7A9D8F]"
                />
              </div>

              {/* Finalidade */}
              <div>
                <label
                  htmlFor="select-finalidade"
                  className="text-xl text-[#2F4F4F] mb-2"
                >
                  Finalidade
                </label>
                <select
                  id="select-finalidade"
                  value={finalidade}
                  onChange={(e) =>
                    setFinalidade(
                      e.target.value === "" ? "" : Number(e.target.value),
                    )
                  }
                  className="w-full border p-2 rounded border-[#9DB4AB] bg-white focus:outline-none focus:border-[#7A9D8F]"
                >
                  <option value="">Selecione a Finalidade</option>
                  <option value={Finalidade.Despesa}>Despesa</option>
                  <option value={Finalidade.Receita}>Receita</option>
                  <option value={Finalidade.Ambas}>Ambas</option>
                </select>
              </div>

              {/* Espaço vazio */}
              <div></div>

              {/* Botão */}
              <div className="flex items-end">
                <Button
                  onClick={CriarCategoria}
                  label="Criar Categoria"
                ></Button>
              </div>
            </div>
          </div>
        </div>

        {/* Card de Listar Categorias */}
        <div className="mt-6 bg-[#F5F7F6] p-6 rounded-xl mx-auto w-290 h-130 border border-black/5 shadow-[0_4px_14px_rgba(0,0,0,0.08)] transition-shadow duration-200 ease-out hover:shadow-[0_8px_20px_rgba(0,0,0,0.12)]">
          <h2 className="text-3xl font-semibold mb-4 text-[#2F4F4F]">
            Lista de Categorias
          </h2>
          <hr className="mb-4 border-[#9DB4AB]"></hr>

          <div className="ml-3 mb-2 grid grid-cols-3 gap-22 w-250">
            <h3 className="text-xl text-[#2F4F4F]">Descrição</h3>
            <h3 className="text-xl text-[#2F4F4F]">Finalidade</h3>
            <h3 className="text-xl ml-50 text-[#2F4F4F]">Ações</h3>
          </div>

          {categorias.length > 0 ? (
            <div className="space-y-3 h-80">
              {categoriasPaginadas.map((categoria) => (
                <div
                  key={categoria.id}
                  className="grid grid-cols-3 gap-4 items-center p-4 border border-[#C8D6D1] rounded-lg bg-white hover:bg-[#E8EFED] transition"
                >
                  <p className="font-medium text-[#2F4F4F]">
                    {categoria.descricao}
                  </p>
                  <p className="text-[#5A7067]">
                    {Finalidade[categoria.finalidade]}
                  </p>

                  <div className="flex gap-2 ml-35">
                    <Button
                      onClick={() => abrirModalEditar(categoria)}
                      label="Editar"
                      variant="edit"
                    ></Button>
                    <Button
                      className="px-3 py-2 bg-[#AD675C] hover:bg-[#6d2a21] text-white rounded"
                      onClick={() => DeletarCategoria(categoria.id)}
                      label="Excluir"
                      variant="delete"
                    ></Button>
                  </div>
                </div>
              ))}
            </div>
          ) : (
            <p className="text-[#89A49D] text-center">
              Nenhuma categoria encontrada
            </p>
          )}

          <div className="flex justify-center gap-2 mt-6">
            {Array.from({ length: totalPaginas }).map((_, index) => {
              const pagina = index + 1;
              return (
                <button
                  key={pagina}
                  onClick={() => setPaginaAtual(pagina)}
                  className={`px-3 py-1 rounded ${
                    paginaAtual === pagina
                      ? "bg-[#2F4F4F] text-white"
                      : "bg-[#D4E2DC] hover:bg-[#C8D6D1] text-[#2F4F4F]"
                  }`}
                >
                  {pagina}
                </button>
              );
            })}
          </div>
        </div>
      </div>

      {/* Modal de Edição */}
      {showModal && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
          <div className="bg-white p-6 rounded-lg w-96 border border-[#9DB4AB]">
            <h2 className="text-2xl font-semibold mb-4 text-[#2F4F4F]">
              Editar Categoria
            </h2>

            <div className="mb-4">
              <label
                htmlFor="input-descricao"
                className="block mb-1 text-[#2F4F4F]"
              >
                Descrição
              </label>
              <input
                id="input-descricao"
                type="text"
                value={descricaoEdit}
                onChange={(e) => setDescricaoEdit(e.target.value)}
                className="w-full border border-[#9DB4AB] p-2 rounded focus:outline-none focus:border-[#7A9D8F]"
              />
            </div>

            <div className="mb-4">
              <label
                htmlFor="select-finalidade"
                className="block mb-1 text-[#2F4F4F]"
              >
                Finalidade
              </label>
              <select
                id="select-finalidade"
                value={finalidadeEdit}
                onChange={(e) =>
                  setFinalidadeEdit(
                    e.target.value === "" ? "" : Number(e.target.value),
                  )
                }
                className="w-full border border-[#9DB4AB] p-2 rounded focus:outline-none focus:border-[#7A9D8F]"
              >
                <option value="">Selecione</option>
                <option value={Finalidade.Despesa}>Despesa</option>
                <option value={Finalidade.Receita}>Receita</option>
                <option value={Finalidade.Ambas}>Ambas</option>
              </select>
            </div>

            <div className="flex justify-center gap-2">
              <button
                onClick={editarCategoria}
                className="px-4 py-2 bg-[#7A9D8F] text-white rounded hover:bg-[#5A7067]"
              >
                Salvar
              </button>
              <button
                onClick={() => setShowModal(false)}
                className="px-4 py-2 bg-[#C8D6D1] text-[#2F4F4F] rounded hover:bg-[#9DB4AB]"
              >
                Cancelar
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
