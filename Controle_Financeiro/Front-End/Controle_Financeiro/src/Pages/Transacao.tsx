import { useEffect, useState } from "react";
import {
  criarTransacao,
  listarTransacoes,
  deletarTransacao,
  alterarTransacao,
  type TransacaoRequest,
} from "../Services/TransacaoService";
import { consultarUsuario } from "../Services/UsuarioService";
import { consultarCategoria } from "../Services/CategoriaService";
import trashCan from "../assets/trash.png";
import pencil from "../assets/pencil.png";
import type { UsuarioResponse } from "../Types/UsuarioResponse";
import type { CategoriaResponse } from "../Types/CategoriaResponse";
import type { TransacaoResponse } from "../Types/TransacaoResponse";

export default function Transacao() {
  const [descricao, setDescricao] = useState("");
  const [valor, setValor] = useState<string>("");
  const [tipo, setTipo] = useState<number | "">("");
  const [categoriaId, setCategoriaId] = useState<number | "">("");
  const [usuarioId, setUsuarioId] = useState<number | "">("");
  const [data, setData] = useState<string>("");
  const [transacoes, setTransacoes] = useState<TransacaoResponse[]>([]);
  const [paginaAtual, setPaginaAtual] = useState(1);
  const [showModal, setShowModal] = useState(false);
  const [usuarios, setUsuarios] = useState<UsuarioResponse[]>([]);
  const [categorias, setCategorias] = useState<CategoriaResponse[]>([]);

  const [descricaoEdit, setDescricaoEdit] = useState("");
  const [valorEdit, setValorEdit] = useState<string>("");
  const [tipoEdit, setTipoEdit] = useState<number | "">("");
  const [categoriaIdEdit, setCategoriaIdEdit] = useState<number | "">("");
  const [usuarioIdEdit, setUsuarioIdEdit] = useState<number | "">("");
  const [dataEdit, setDataEdit] = useState<string>("");

  const transacoesPorPagina = 4;
  const totalPaginas = Math.ceil(transacoes.length / transacoesPorPagina);
  const indiceInicial = (paginaAtual - 1) * transacoesPorPagina;
  const indiceFinal = indiceInicial + transacoesPorPagina;
  const transacoesPaginadas = transacoes.slice(indiceInicial, indiceFinal);

  const [transacaoSelecionada, setTransacaoSelecionada] =
    useState<TransacaoResponse | null>(null);

  // Verificar se usuário é menor de idade
  const usuarioAtual = showModal ? usuarioIdEdit : usuarioId;
  const usuarioSelecionado = usuarios.find((u) => u.id === usuarioAtual);
  const isMenorDeIdade = usuarioSelecionado
    ? usuarioSelecionado.idade < 18
    : false;

  const tipoAtual = showModal ? tipoEdit : tipo;

  const categoriasFiltradas = categorias.filter((categoria) => {
    if (tipoAtual === "") return false;

    if (tipoAtual === 1) {
      return categoria.finalidade === 2 || categoria.finalidade === 3;
    } else if (tipoAtual === 2) {
      return categoria.finalidade === 1 || categoria.finalidade === 3;
    }
    return false;
  });

  async function CriarTransacao() {
    try {
      if (
        !descricao ||
        valor === "" ||
        tipo === "" ||
        categoriaId === "" ||
        usuarioId === "" ||
        !data
      ) {
        alert("Preencha todos os campos");
        return;
      }

      const valorNumerico = Number(valor.replace(",", "."));
      if (isNaN(valorNumerico) || valorNumerico <= 0) {
        alert("Valor inválido");
        return;
      }
      const dataISO = new Date(data).toISOString();
      const payload: TransacaoRequest = {
        descricao,
        valor: valorNumerico,
        tipo: Number(tipo),
        categoriaId: Number(categoriaId),
        usuarioId: Number(usuarioId),
        data: dataISO,
      };

      const transacao = await criarTransacao(payload);
      console.log("Transação criada com sucesso:", transacao);
      await listarTodasTransacoes();
      alert("Transação criada com sucesso!");

      // Limpar campos
      setDescricao("");
      setValor("");
      setTipo("");
      setCategoriaId("");
      setUsuarioId("");
      setData("");
    } catch (error) {
      console.error(error);
      alert("Erro ao criar transação");
    }
  }

  async function listarTodasTransacoes() {
    try {
      const transacoesData = await listarTransacoes();
      setTransacoes(transacoesData);
      console.log("Transações Existentes:", transacoesData);
    } catch (error) {
      console.error(error);
      alert("Erro ao consultar a lista de transações");
    }
  }

  async function listarUsuarios() {
    try {
      const usuariosData = await consultarUsuario();
      setUsuarios(usuariosData);
      console.log("Usuários Existentes:", usuariosData);
    } catch (error) {
      console.error(error);
      alert("Erro ao consultar a lista de usuários");
    }
  }

  async function listarCategorias() {
    try {
      const categoriasData = await consultarCategoria();
      setCategorias(categoriasData);
      console.log("Categorias Existentes:", categoriasData);
    } catch (error) {
      console.error(error);
      alert("Erro ao consultar a lista de categorias");
    }
  }

  async function DeletarTransacao(id: number) {
    const confirmar = window.confirm("Tem certeza que deseja excluir?");
    if (!confirmar) return;
    try {
      await deletarTransacao(id);
      await listarTodasTransacoes();
      console.log("Transação Deletada com Sucesso:", id);
    } catch (error) {
      console.error(error);
      alert("Erro ao excluir a transação");
    }
  }

  async function editarTransacao() {
    if (!transacaoSelecionada) return;

    try {
      const valorNumerico = Number(valorEdit.replace(",", "."));

      await alterarTransacao(transacaoSelecionada.id, {
        descricao: descricaoEdit,
        valor: valorNumerico,
        tipo: Number(tipoEdit),
        categoriaId: Number(categoriaIdEdit),
        usuarioId: Number(usuarioIdEdit),
        data: dataEdit,
      });

      await listarTodasTransacoes();
      alert("Transação alterada com sucesso!");
      setShowModal(false);
    } catch (error) {
      console.error(error);
      alert("Erro ao alterar transação");
    }
  }

  useEffect(() => {
    const carregarDados = async () => {
      await listarTodasTransacoes();
      await listarUsuarios();
      await listarCategorias();
    };

    carregarDados();
  }, []);

  function abrirModalEditar(transacao: TransacaoResponse) {
    setTransacaoSelecionada(transacao);
    setDescricaoEdit(transacao.descricao);

    const valorFormatado = transacao.valor.toFixed(2).replace(".", ",");
    setValorEdit(valorFormatado);

    setTipoEdit(transacao.tipo);
    setCategoriaIdEdit(transacao.categoriaId);
    setUsuarioIdEdit(transacao.usuarioId);

    // formata data para o input
    const dataFormatada = transacao.data.split("T")[0];
    setDataEdit(dataFormatada);

    setShowModal(true);
  }

  function getTipoLabel(tipo: number): string {
    return tipo === 1 ? "Receita" : "Despesa";
  }

  //  adicionar o símbolo
  function formatarValor(valor: number, tipo: number): string {
    const sinal = tipo === 1 ? "+ " : "- ";
    const valorFormatado = valor.toLocaleString("pt-BR", {
      style: "currency",
      currency: "BRL",
    });
    return sinal + valorFormatado;
  }

  //  formatar data só para mostrar na tela
  function formatarData(dataISO: string): string {
    {
      console.log("Data da transação:", dataISO);
    }
    if (!dataISO) return "Data inválida";

    const [ano, mes, dia] = dataISO.split("T")[0].split("-");
    return `${dia}/${mes}/${ano}`;
  }
  return (
    <div className="">
      <div className="flex justify-center gap-4 p-6 backdrop-blur-md rounded-lg"></div>

      <div className="flex flex-col">
        {/* Card de Criar Transação */}
        <div className="mt-6 bg-[#F5F7F6] rounded-lg p-6 w-290 mx-auto shadow-[0_4px_14px_rgba(0,0,0,0.08)] border border-black/5 transition hover:shadow-[0_8px_20px_rgba(0,0,0,0.12)]">
          <h2 className="text-3xl font-semibold mb-4 text-[#2F4F4F]">
            Adicionar Nova Transação
          </h2>
          <hr className="mb-4 border-[#9DB4AB]"></hr>

          <div className="flex flex-col gap-4">
            <div className="grid grid-cols-2 gap-2">
              {/* Descrição */}
              <div>

                <label
                  htmlFor="select-descricao"
                  className="block text-xl text-[#2F4F4F] mb-2"
                >
                  Descricão
                </label>
                <input
                  type="text"
                  placeholder="Digite a descrição"
                  value={descricao}
                  onChange={(e) => setDescricao(e.target.value)}
                  className="w-full rounded border border-[#9DB4AB] bg-white p-2 focus:outline-none focus:border-[#7A9D8F]"
                />
              </div>

              {/* Valor */}
              <div>
                <label
                  htmlFor="select-valor"
                  className="block text-xl text-[#2F4F4F] mb-2"
                >
                  Valor
                </label>
                <input
                  id="select-valor"
                  type="text"
                  placeholder="R$ 100,00"
                  value={valor}
                  onChange={(e) => {
                    const apenasNumeros = e.target.value.replace(/\D/g, "");

                    if (apenasNumeros === "") {
                      setValor("");
                    } else {
                      const numeroFormatado = (
                        Number(apenasNumeros) / 100
                      ).toFixed(2);
                      const valorComVirgula = numeroFormatado.replace(".", ",");
                      setValor(valorComVirgula);
                    }
                  }}
                  className="w-full border p-2 rounded border-[#9DB4AB] bg-white focus:outline-none focus:border-[#7A9D8F]"
                />
              </div>

              {/* Usuário */}
              <div>
                <label
                  htmlFor="select-usuario"
                  className=" block text-xl text-[#2F4F4F] mb-2"
                >
                  Usuário
                </label>
                <select
                  id="select-usuario"
                  value={usuarioId}
                  onChange={(e) => {
                    const novoUsuarioId =
                      e.target.value === "" ? "" : Number(e.target.value);
                    setUsuarioId(novoUsuarioId);

                    // Limpa tipo e categoria ao trocar de usuário
                    setTipo("");
                    setCategoriaId("");
                  }}
                  className="w-full border p-2 rounded border-[#9DB4AB] bg-white focus:outline-none focus:border-[#7A9D8F]"
                >
                  <option value="">Selecione o usuário</option>
                  {usuarios.map((usuario) => (
                    <option key={usuario.id} value={usuario.id}>
                      {usuario.nome} ({usuario.idade} anos)
                    </option>
                  ))}
                </select>
              </div>

              {/* Data */}
              <div>
                <label
                  htmlFor="select-data"
                  className="block text-xl text-[#2F4F4F] mb-2"
                >
                  Data
                </label>
                <input
                  id="select-data"
                  type="date"
                  value={data}
                  onChange={(e) => setData(e.target.value)}
                  className="w-full border p-2 rounded border-[#9DB4AB] bg-white focus:outline-none focus:border-[#7A9D8F]"
                />
              </div>



              {/* Tipo */}
              <div>
                <label
                  htmlFor="select-tipo"
                  className="block text-xl text-[#2F4F4F] mb-2"
                >
                  Tipo
                </label>
                <select
                  id="select-tipo"
                  value={tipo}
                  onChange={(e) => {
                    const novoTipo =
                      e.target.value === "" ? "" : Number(e.target.value);
                    setTipo(novoTipo);

                    // Limpa categoria ao trocar de tipo
                    setCategoriaId("");
                  }}
                  disabled={usuarioId === ""}
                  className={`w-full border p-2 rounded border-[#9DB4AB] bg-white focus:outline-none focus:border-[#7A9D8F] ${usuarioId === "" ? "opacity-50 cursor-not-allowed" : ""
                    }`}
                >
                  <option value="">Selecione o tipo</option>
                  {!isMenorDeIdade && <option value="1">Receita</option>}
                  <option value="2">Despesa</option>
                </select>
                {isMenorDeIdade && usuarioId !== "" && (
                  <p className="text-sm text-orange-600 mt-1">
                    ⚠️ Menor de 18 anos - Apenas despesas permitidas
                  </p>
                )}
              </div>

              {/* Categoria */}
              <div>
                <label
                  htmlFor="select-categoria"
                  className="block text-xl text-[#2F4F4F] mb-2"
                >
                  Categoria
                </label>
                <select
                  id="select-categoria"
                  value={categoriaId}
                  onChange={(e) =>
                    setCategoriaId(
                      e.target.value === "" ? "" : Number(e.target.value),
                    )
                  }
                  disabled={tipo === ""}
                  className={`w-full border p-2 rounded border-[#9DB4AB] bg-white focus:outline-none focus:border-[#7A9D8F] ${tipo === "" ? "opacity-50 cursor-not-allowed" : ""
                    }`}
                >
                  <option value="">Selecione a categoria</option>
                  {categoriasFiltradas.map((categoria) => (
                    <option key={categoria.id} value={categoria.id}>
                      {categoria.descricao}
                    </option>
                  ))}
                </select>
              </div>

              {/* Botão */}
              <div className="flex items-end">
                <button
                  onClick={CriarTransacao}
                  className="bg-[#7A9D8F] text-white w-full h-11 rounded hover:bg-[#5A7067] transition"
                >
                  Criar Transação
                </button>
              </div>
            </div>
          </div>
        </div>

        {/* Card de Listar Transações */}
        <div className="mt-6 bg-[#F5F7F6] p-6 rounded-xl mx-auto w-290 h-130 border border-black/5 shadow-[0_4px_14px_rgba(0,0,0,0.08)] transition-shadow duration-200 ease-out hover:shadow-[0_8px_20px_rgba(0,0,0,0.12)]">
          <h2 className="text-3xl font-semibold mb-4 text-[#2F4F4F]">
            Lista de Transações
          </h2>
          <hr className="mb-4 border-[#9DB4AB]"></hr>

          <div className="mb-2 grid grid-cols-[1fr_1fr_1fr_1fr_1fr_1fr_2fr] gap-4 px-4">
            <h3 className="text-xl text-[#2F4F4F]">Usuário</h3>
            <h3 className="text-xl text-[#2F4F4F]">Descrição</h3>
            <h3 className="text-xl text-[#2F4F4F]">Data</h3>
            <h3 className="text-xl text-[#2F4F4F]">Valor</h3>
            <h3 className="text-xl text-[#2F4F4F]">Tipo</h3>
            <h3 className="text-xl text-[#2F4F4F]">Categoria</h3>
            <h3 className="text-xl ml-16 text-[#2F4F4F]">Ações</h3>
          </div>

          {transacoes.length > 0 ? (
            <div className="space-y-3 h-80">
              {transacoesPaginadas.map((transacao) => (
                <div
                  key={transacao.id}
                  className="grid grid-cols-[1fr_1fr_1fr_1fr_1fr_1fr_2fr] gap-4 items-center p-4 border border-[#C8D6D1] rounded-lg bg-white hover:bg-[#E8EFED] transition"
                >
                  <p className="font-medium text-[#2F4F4F]">
                    {transacao.usuarioNome}
                  </p>

                  <p
                    className="font-medium text-[#2F4F4F] truncate"
                    title={transacao.descricao}
                  >
                    {transacao.descricao}
                  </p>

                  <p className="text-[#5A7067]">
                    {formatarData(transacao.data)}
                  </p>

                  <p
                    className={`font-medium ${transacao.tipo === 1 ? "text-green-700" : "text-red-700"}`}
                  >
                    {formatarValor(transacao.valor, transacao.tipo)}
                  </p>

                  <p
                    className={`font-medium ${transacao.tipo === 1 ? "text-green-600" : "text-red-600"}`}
                  >
                    {getTipoLabel(transacao.tipo)}
                  </p>

                  <p className="text-[#5A7067]">
                    {transacao.categoriaDescricao}
                  </p>

                  <div className="flex gap-2">
                    <button
                      onClick={() => abrirModalEditar(transacao)}
                      className="px-3 py-2 bg-[#7A9D8F] hover:bg-[#5A7067] text-white rounded"
                    >
                      <div className="flex items-center gap-1">
                        <img src={pencil} alt="Editar" className="w-5 h-5" />
                        Editar
                      </div>
                    </button>
                    <button
                      className="px-3 py-2 bg-[#AD675C] hover:bg-[#6d2a21] text-white rounded"
                      onClick={() => DeletarTransacao(transacao.id)}
                    >
                      <div className="flex items-center gap-1">
                        <img src={trashCan} alt="Excluir" className="w-5 h-5" />
                        Excluir
                      </div>
                    </button>
                  </div>
                </div>
              ))}
            </div>
          ) : (
            <p className="text-[#89A49D] text-center">
              Nenhuma transação encontrada
            </p>
          )}

          <div className="flex justify-center gap-2 mt-6">
            {Array.from({ length: totalPaginas }).map((_, index) => {
              const pagina = index + 1;
              return (
                <button
                  key={pagina}
                  onClick={() => setPaginaAtual(pagina)}
                  className={`px-3 py-1 rounded ${paginaAtual === pagina
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
              Editar Transação
            </h2>

            <div className="mb-4">
              <label
                htmlFor="select-descricao"
                className="block mb-1 text-[#2F4F4F]"
              >
                Descrição
              </label>
              <input
                id="select-descricao"
                type="text"
                value={descricaoEdit}
                onChange={(e) => setDescricaoEdit(e.target.value)}
                className="w-full border border-[#9DB4AB] p-2 rounded focus:outline-none focus:border-[#7A9D8F]"
              />
            </div>

            <div className="mb-4">
              <label className="block mb-1 text-[#2F4F4F]">Valor</label>
              <input
                type="text"
                placeholder="R$ 100,00"
                value={valorEdit}
                onChange={(e) => {
                  const apenasNumeros = e.target.value.replace(/\D/g, "");
                  if (apenasNumeros === "") {
                    setValorEdit("");
                  } else {
                    const numeroFormatado = (
                      Number(apenasNumeros) / 100
                    ).toFixed(2);
                    const valorComVirgula = numeroFormatado.replace(".", ",");
                    setValorEdit(valorComVirgula);
                  }
                }}
                className="w-full border border-[#9DB4AB] p-2 rounded focus:outline-none focus:border-[#7A9D8F]"
              />
            </div>

            <div className="mb-4">
              <label
                htmlFor="select-data"
                className="block mb-1 text-[#2F4F4F]"
              >
                Data
              </label>
              <input
                id="select-data"
                type="date"
                value={dataEdit}
                onChange={(e) => setDataEdit(e.target.value)}
                className="w-full border border-[#9DB4AB] p-2 rounded focus:outline-none focus:border-[#7A9D8F]"
              />
            </div>

            <div className="mb-4">
              <label
                htmlFor="select-usuario"
                className="block mb-1 text-[#2F4F4F]"
              >
                Usuário
              </label>
              <select
                id="select-usuario"
                value={usuarioIdEdit}
                onChange={(e) =>
                  setUsuarioIdEdit(
                    e.target.value === "" ? "" : Number(e.target.value),
                  )
                }
                className="w-full border border-[#9DB4AB] p-2 rounded focus:outline-none focus:border-[#7A9D8F]"
              >
                <option value="">Selecione o usuário</option>
                {usuarios.map((usuario) => (
                  <option key={usuario.id} value={usuario.id}>
                    {usuario.nome}
                  </option>
                ))}
              </select>
            </div>

            <div className="mb-4">
              <label
                htmlFor="select-tipo"
                className="block mb-1 text-[#2F4F4F]"
              >
                Tipo
              </label>
              <select
                id="select-tipo"
                value={tipoEdit}
                onChange={(e) => {
                  setTipoEdit(
                    e.target.value === "" ? "" : Number(e.target.value),
                  );
                  setCategoriaIdEdit("");
                }}
                disabled={usuarioIdEdit === ""}
                className={`w-full border border-[#9DB4AB] p-2 rounded focus:outline-none focus:border-[#7A9D8F] ${usuarioIdEdit === "" ? "opacity-50 cursor-not-allowed" : ""
                  }`}
              >
                <option value="">Selecione o tipo</option>
                {!isMenorDeIdade && <option value="1">Receita</option>}
                <option value="2">Despesa</option>
              </select>
              {isMenorDeIdade && usuarioIdEdit !== "" && (
                <p className="text-sm text-orange-600 mt-1">
                  ⚠️ Menor de 18 anos - Apenas despesas permitidas
                </p>
              )}
            </div>

            <div className="mb-4">
              <label
                htmlFor="select-categoria"
                className="block mb-1 text-[#2F4F4F]"
              >
                Categoria
              </label>
              <select
                id="select-categoria"
                value={categoriaIdEdit}
                onChange={(e) =>
                  setCategoriaIdEdit(
                    e.target.value === "" ? "" : Number(e.target.value),
                  )
                }
                className="w-full border border-[#9DB4AB] p-2 rounded focus:outline-none focus:border-[#7A9D8F]"
              >
                <option value="">Selecione a categoria</option>
                {categorias.map((categoria) => (
                  <option key={categoria.id} value={categoria.id}>
                    {categoria.descricao}
                  </option>
                ))}
              </select>
            </div>

            <div className="flex justify-end gap-2">
              <button
                onClick={() => setShowModal(false)}
                className="px-4 py-2 bg-[#C8D6D1] text-[#2F4F4F] rounded hover:bg-[#9DB4AB]"
              >
                Cancelar
              </button>

              <button
                onClick={editarTransacao}
                className="px-4 py-2 bg-[#7A9D8F] text-white rounded hover:bg-[#5A7067]"
              >
                Salvar
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
