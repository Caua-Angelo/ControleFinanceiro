import { useState } from "react";
import { useNavigate } from "react-router-dom";
import toast from "react-hot-toast";
import { Register } from "../Services/AuthService";
import axios from "axios";

export function RegisterPage() {
  const [nome, setNome] = useState("");
  const [dataNascimento, setDataNascimento] = useState("");
  const [email, setEmail] = useState("");
  const [senha, setSenha] = useState("");
  const [isLoading, setIsLoading] = useState(false);

  const navigate = useNavigate();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsLoading(true);

    try {
      await Register(nome, dataNascimento, email, senha);

      toast.success("Cadastro realizado com sucesso!");
      navigate("/transacao");
    } catch (error) {
      if (axios.isAxiosError(error)) {
        toast.error(error.response?.data?.mensagem ?? "Erro ao realizar o cadastro");
      } else {
        toast.error("Erro ao realizar o cadastro");
      }
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-[#9DB4AB]">
      <div className="w-full max-w-md bg-white rounded-xl shadow-lg p-6">
        <div className="text-center mb-6">
          <h2 className="text-2xl font-semibold text-gray-800">Registrar-se</h2>
          <p className="text-gray-500 text-sm">Crie sua conta para continuar</p>
        </div>

        <form onSubmit={handleSubmit} className="space-y-5">
          {/* Nome */}
          <div>
            <label className="block text-sm text-gray-700 mb-1">Nome</label>
            <input
              type="text"
              placeholder="Nome"
              value={nome}
              onChange={(e) => setNome(e.target.value)}
              className="w-full border rounded px-3 py-2 focus:outline-none focus:ring-2 focus:ring-[#9DB4AB]"
              required
            />
          </div>

          {/* Data de Nascimento */}
          <div>
            <label className="block text-sm text-gray-700 mb-1">Data de Nascimento</label>
            <input
              type="date"
              placeholder="Data de Nascimento"
              value={dataNascimento}
              onChange={(e) => setDataNascimento(e.target.value)}
              max={new Date().toISOString().split("T")[0]}
              className="w-full border rounded px-3 py-2 focus:outline-none focus:ring-2 focus:ring-[#9DB4AB]"
              required
            />
          </div>

          {/* Email */}
          <div>
            <label className="block text-sm text-gray-700 mb-1">Email</label>
            <input
              type="email"
              placeholder="Email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              className="w-full border rounded px-3 py-2 focus:outline-none focus:ring-2 focus:ring-[#9DB4AB]"
              required
            />
          </div>

          {/* Senha */}
          <div>
            <label className="block text-sm text-gray-700 mb-1">Senha</label>
            <input
              type="password"
              placeholder="••••••••"
              value={senha}
              onChange={(e) => setSenha(e.target.value)}
              minLength={8}
              className="w-full border rounded px-3 py-2 focus:outline-none focus:ring-2 focus:ring-[#9DB4AB]"
              required
            />
            <p className="text-xs text-gray-500 mt-1">A senha deve ter no mínimo 8 caracteres.</p>
          </div>
          <div>
            {/* Botão de registrar */}
            <button
              type="submit"
              className="w-full bg-[#7A9D8F] text-white py-2 rounded hover:bg-[#5A7067] transition disabled:opacity-50 disabled:cursor-not-allowed"
              disabled={isLoading}
            >
              {isLoading ? "Registrando..." : "Registrar"}
            </button>
          </div>
          <p className="text-center text-sm text-gray-600 mt-4">
            Já possui uma conta?{" "}
            <button type="button" onClick={() => navigate("/login")} className="text-[#7A9D8F] hover:underline">
              Entrar
            </button>
          </p>
        </form>
      </div>
    </div>
  );
}
