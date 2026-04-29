import { api } from "./ApiService";

export interface UsuarioUpdateRequest {
  nome: string;
  idade: number;
}

export async function getUsuarioLogado() {
  const response = await api.get("/usuarios/me");
  return response.data;
}

export async function atualizarUsuario(data: UsuarioUpdateRequest) {
  const response = await api.put("/usuarios/me", data);
  return response.data;
}

export async function deletarConta() {
  await api.delete("/usuarios/me");
}
