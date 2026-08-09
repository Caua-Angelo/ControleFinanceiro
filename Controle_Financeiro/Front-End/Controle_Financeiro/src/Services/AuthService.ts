import { api } from "./ApiService";

export async function login(email: string, senha: string) {
  const response = await api.post("/auth/login", {
    email,
    senha,
  });

  const token = response.data.token;

  localStorage.setItem("token", token);

  return token;
}
export async function Register(nome: string, dataNascimento: string, email: string, senha: string) {
  const response = await api.post("/auth/register", {
    nome,
    dataNascimento,
    email,
    senha,
  });
  return response;
}

export function logout() {
  localStorage.removeItem("token");
}
