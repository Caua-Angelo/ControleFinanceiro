import React, { useState } from 'react';
import { criarUsuario } from "../Services/UsuarioService";

type Acao = "criar" | "editar" | "excluir" | null;
export default function Usuario() {
  const [acao, setAcao] = useState<Acao>(null);
  const [nome, setNome] = useState("");
  const [idade, setIdade] = useState<number | "">("");

  async function CriarUsuario() {
    if (!nome || idade === "") {
      alert("Preencha todos os campos");
      return;
    }

    const payload = {
      nome,
      idade: Number(idade),
    };

    await criarUsuario(payload);
  }

  return (
    <div className=''>
      <h1>Usuário</h1>
      <p>Aqui você pode gerenciar os usuários do sistema.</p>
      <div className="flex justify-center gap-4 bg-blue-300 p-6 rounded-lg">
        <button onClick={() => setAcao("criar")} className="px-4 py-2 bg-green-600 text-white rounded hover:bg-green-700 transition">
          Criar
        </button>

        <button onClick={() => setAcao("editar")} className="px-4 py-2 bg-yellow-500 text-white rounded hover:bg-yellow-600 transition">
          Editar
        </button>

        <button onClick={() => setAcao("excluir")} className="px-4 py-2 bg-red-600 text-white rounded hover:bg-red-700 transition">
          Excluir
        </button>
      </div>
      {/* Conteúdo dinâmico */}
      {acao === "criar" && (
        <div className="mt-6 bg-white p-6 rounded-lg shadow-md w-full max-w-md mx-auto">
          <h2 className="text-xl font-semibold mb-4">Criar usuário</h2>

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
              className="border p-2 rounded"
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
      {acao === "editar" && (<div>✏️ Tela de edição</div>)}
      {acao === "excluir" && (<div>🗑️ Confirmação de exclusão</div>)}

    </div>
  );
}
