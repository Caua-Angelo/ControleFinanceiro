import React, { useState } from 'react';
import { criarUsuario, ListarUsuario } from "../Services/UsuarioService";

type Acao = "criar" | "editar" | "excluir" | null;
export default function Usuario() {
  const [acao, setAcao] = useState<Acao>(null);
  const [nome, setNome] = useState("");
  const [, setUsuarioCriado] = useState<UsuarioResponse | null>(null);
  const [usuarios, setUsuarios] = useState<UsuarioResponse[]>([]);

  const [idade, setIdade] = useState<number | "">("");

  interface UsuarioResponse {
    id: number;
    nome: string;
    idade: number;
  }

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
      setUsuarioCriado(usuario);
      console.log("Usuário criado com sucesso:", usuario);
      alert("Usuário criado com sucesso!");
    } catch (error) {
      console.error(error);
      alert("Erro ao criar usuário");
    }
  }

  async function listarUsuarios() {
    try {

      const usuariosData = await ListarUsuario();
      setUsuarios(usuariosData);
      console.log("Usuário Existentes:", usuariosData);

    } catch (error) {
      console.error(error);
      alert("Erro ao consultar a lista de usuários usuário");
    }
  }

  return (
    <div className=''>

      <div className=' items-center flex flex-col mb-6'>
        <h1 className="text-2xl font-bold"> Sessão do usuário </h1>
        <p className="text-2xl font-bold">Aqui você pode gerenciar os usuários do sistema.</p>
      </div>
      <div className="flex justify-center  gap-4 bg-white p-6 backdrop-blur-md rounded-lg">
        <button onClick={() => setAcao("criar")} className="px-4 py-2 bg-[#5F9EA0] text-white rounded hover:bg-[#2F4F4F] transition">
          Criar
        </button>

        <button
          onClick={async () => {
            setAcao("editar");
            await listarUsuarios();
          }}
          className="px-4 py-2 bg-[#5F9EA0] text-white rounded hover:bg-[#2F4F4F] transition"
        >
          Editar
        </button>

        <button onClick={() => setAcao("excluir")} className="px-4 py-2 bg-[#5F9EA0] text-white rounded hover:bg-[#2F4F4F] transition">
          Excluir
        </button>
      </div>
      {/* Conteúdo dinâmico */}
      {acao === "criar" && (
        <div className="mt-6 bg-white p-6 rounded-lg backdrop-blur-sm w-full max-w-md mx-auto">
          <h2 className="text-xl  font-semibold mb-4">Criar usuário</h2>

          <div className="flex flex-col gap-4">
            {/* Nome */}
            <input
              type="text"
              placeholder="Nome"
              value={nome}
              onChange={(e) => setNome(e.target.value)}
              className="border p-2 rounded"
            />

            {/* Idade */}
            <input
              type="number"
              placeholder="Idade"
              value={idade}
              onChange={(e) =>
                setIdade(e.target.value === "" ? "" : Number(e.target.value))
              }
              className=" border p-2 rounded"
            />

            <button
              onClick={CriarUsuario}
              className="bg-green-600 text-white py-2 rounded hover:bg-green-700 transition"
            >
              Salvar
            </button>
          </div>
        </div>
      )}
      {acao === "editar" && (
        <div className="mt-6 bg-white p-6 rounded-lg shadow-md w-full max-w-2xl mx-auto">
          <h2 className="text-xl font-semibold mb-4">Editar usuário</h2>

          {usuarios.length > 0 ? (
            <div className="space-y-3">
              {usuarios.map((user) => (
                <div
                  key={user.id}
                  className="flex justify-between items-center p-4 border rounded-lg hover:bg-gray-50 transition"
                >
                  <div>
                    <p className="font-semibold text-lg">{user.nome}</p>
                    <p className="text-sm text-gray-600">{user.idade} anos</p>
                  </div>
                  <button className="px-4 py-2 bg-blue-500 text-white rounded hover:bg-blue-600 transition">
                    Editar
                  </button>
                </div>
              ))}
            </div>
          ) : (
            <p className="text-gray-500 text-center">Nenhum usuário encontrado</p>
          )}
        </div>
      )}






      {acao === "excluir" && (
        <div>🗑️ Confirmação de exclusão</div>)}

    </div>
  );
}
