import { useEffect, useState } from "react";
import { criarUsuario, consultarUsuario, deletarUsuario, alterarUsuario } from "../Services/UsuarioService";
import type { UsuarioResponse } from "../Types/UsuarioResponse";
import { Button } from "../Components/Button";
export default function Usuario() {
  const [nome, setNome] = useState("");
  const [usuarios, setUsuarios] = useState<UsuarioResponse[]>([]);
  const [idade, setIdade] = useState<number | "">("");
  const [paginaAtual, setPaginaAtual] = useState(1);
  const [showModal, setShowModal] = useState(false);

  const [idadeEdit, setIdadeEdit] = useState<number | "">("");
  const [nomeEdit, setNomeEdit] = useState("");

  const usuariosPorPagina = 4;
  const totalPaginas = Math.ceil(usuarios.length / usuariosPorPagina);
  const indiceInicial = (paginaAtual - 1) * usuariosPorPagina;
  const indiceFinal = indiceInicial + usuariosPorPagina;
  const usuariosPaginados = usuarios.slice(indiceInicial, indiceFinal);

  const [usuarioSelecionado, setUsuarioSelecionado] = useState<UsuarioResponse | null>(null);

  async function CriarUsuario() {
    try {
      if (!nome || idade === "") {
        alert("Preencha todos os campos");
        return;
      }
      const payload = {
        nome,
        idade: Number(idade),
      };

      const usuario = await criarUsuario(payload);
      console.log("Usuário criado com sucesso:", usuario);
      await listarUsuarios();
      alert("Usuário criado com sucesso!");

      setNome("");
      setIdade("");
    } catch (error) {
      console.error(error);
      alert("Erro ao criar usuário");
    }
  }

  async function listarUsuarios() {
    try {
      const usuariosData = await consultarUsuario();
      setUsuarios(usuariosData);
      console.log("Usuário Existentes:", usuariosData);
    } catch (error) {
      console.error(error);
      alert("Erro ao consultar a lista de usuários usuário");
    }
  }

  async function DeletarUsuarios(id: number) {
    const confirmar = window.confirm("Tem certeza que deseja excluir?");
    if (!confirmar) return;
    try {
      await deletarUsuario(id);
      await listarUsuarios();
      console.log("Usuário Deletado com Sucesso:", id);
    } catch (error) {
      console.error(error);
      alert("Erro ao excluir o usuário");
    }
  }

  async function editarUsuario() {
    if (!usuarioSelecionado) return;

    try {
      await alterarUsuario(usuarioSelecionado.id, {
        nome: nomeEdit,
        idade: Number(idadeEdit),
      });

      await listarUsuarios();
      alert("Usuário alterado com sucesso!");
      setShowModal(false);
    } catch (error) {
      console.error(error);
      alert("Erro ao alterar usuário");
    }
  }

  useEffect(() => {
    listarUsuarios();
  }, []);

  function abrirModalEditar(usuario: UsuarioResponse) {
    setUsuarioSelecionado(usuario);
    setNomeEdit(usuario.nome);
    setIdadeEdit(usuario.idade);
    setShowModal(true);
  }

  return (
    <div className="">
      <div className="flex justify-center gap-4 p-6 backdrop-blur-md rounded-lg"></div>

      <div className="flex flex-col">
        {/* Card de Criar Usuário */}
        <div className="mt-6 bg-[#F5F7F6] rounded-lg p-6 w-290 mx-auto shadow-[0_4px_14px_rgba(0,0,0,0.08)] border border-black/5 transition hover:shadow-[0_8px_20px_rgba(0,0,0,0.12)]">
          <h2 className="text-3xl font-semibold mb-4 text-[#2F4F4F]">Adicionar Novo Usuário</h2>
          <hr className="mb-4 border-[#9DB4AB]"></hr>

          <div className="flex flex-col gap-4">
            <div className="grid grid-cols-2 gap-4">
              {/* Nome */}
              <div>
                <label htmlFor="input-nome" className="text-xl text-[#2F4F4F] mb-2">
                  Nome Completo
                </label>

                <input
                  id="input-nome"
                  type="text"
                  placeholder="Digite o nome completo"
                  value={nome}
                  onChange={(e) => setNome(e.target.value)}
                  className="w-full rounded border border-[#9DB4AB] bg-white p-2 focus:outline-none focus:border-[#7A9D8F]"
                />
              </div>

              {/* Idade */}
              <div>
                <label className="text-xl text-[#2F4F4F] mb-2">Idade</label>
                <input
                  type="number"
                  placeholder="Ex: 18"
                  value={idade}
                  onChange={(e) => setIdade(e.target.value === "" ? "" : Number(e.target.value))}
                  className="w-full border p-2 rounded border-[#9DB4AB] bg-white focus:outline-none focus:border-[#7A9D8F]"
                />
              </div>

              {/* Espaço vazio */}
              <div></div>

              {/* Botão */}
              <div className="flex items-end">
                <Button onClick={CriarUsuario} label="Criar Usuário"></Button>
              </div>
            </div>
          </div>
        </div>

        {/* Card de Listar Usuários */}
        <div className="mt-6 bg-[#F5F7F6] p-6 rounded-xl mx-auto w-290 h-130 border border-black/5 shadow-[0_4px_14px_rgba(0,0,0,0.08)] transition-shadow duration-200 ease-out hover:shadow-[0_8px_20px_rgba(0,0,0,0.12)]">
          <h2 className="text-3xl font-semibold mb-4 text-[#2F4F4F]">Lista de Usuários</h2>
          <hr className="mb-4 border-[#9DB4AB]"></hr>

          <div className="ml-3 mb-2 grid grid-cols-3 gap-22 w-250">
            <h3 className="text-xl text-[#2F4F4F]">Nome</h3>
            <h3 className="text-xl text-[#2F4F4F]">Idade</h3>
            <h3 className="text-xl ml-50 text-[#2F4F4F]">Ações</h3>
          </div>

          {usuarios.length > 0 ? (
            <div className="space-y-3 h-80">
              {usuariosPaginados.map((user) => (
                <div
                  key={user.id}
                  className="grid grid-cols-3 gap-4 items-center p-4 border border-[#C8D6D1] rounded-lg bg-white hover:bg-[#E8EFED] transition"
                >
                  <p className="font-medium text-[#2F4F4F]">{user.nome}</p>
                  <p className="text-[#5A7067]">{user.idade} anos</p>

                  <div className="flex gap-2 ml-35">
                    <Button onClick={() => abrirModalEditar(user)} variant="edit" label="Editar"></Button>
                    <Button onClick={() => DeletarUsuarios(user.id)} variant="delete" label="Excluir"></Button>
                  </div>
                </div>
              ))}
            </div>
          ) : (
            <p className="text-[#89A49D] text-center">Nenhum usuário encontrado</p>
          )}

          <div className="flex justify-center">
            <Button
              label=""
              variant="paginacao"
              onClick={() => {}}
              paginaAtual={paginaAtual}
              totalPaginas={totalPaginas}
              setPaginaAtual={setPaginaAtual}
            ></Button>
          </div>
        </div>
      </div>

      {/* Modal de Edição */}
      {showModal && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
          <div className="bg-white p-6 rounded-lg w-96 border border-[#9DB4AB]">
            <h2 className="text-2xl font-semibold mb-4 text-[#2F4F4F]">Editar Usuário</h2>

            <div className="mb-4">
              <label htmlFor="input-nome" className="block mb-1 text-[#2F4F4F]">
                Nome
              </label>
              <input
                id="input-nome"
                type="text"
                value={nomeEdit}
                onChange={(e) => setNomeEdit(e.target.value)}
                className="w-full border border-[#9DB4AB] p-2 rounded focus:outline-none focus:border-[#7A9D8F]"
              />
            </div>

            <div className="mb-4">
              <label htmlFor="input-idade" className="block mb-1 text-[#2F4F4F]">
                Idade
              </label>
              <input
                id="input-idade"
                type="number"
                value={idadeEdit}
                onChange={(e) => setIdadeEdit(e.target.value === "" ? "" : Number(e.target.value))}
                className="w-full border border-[#9DB4AB] p-2 rounded focus:outline-none focus:border-[#7A9D8F]"
              />
            </div>

            <div className="flex justify-center gap-2">
              <Button onClick={editarUsuario} label="Salvar" variant="saveModal"></Button>
              <Button onClick={() => setShowModal(false)} label="Cancelar" variant="cancelModal"></Button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
