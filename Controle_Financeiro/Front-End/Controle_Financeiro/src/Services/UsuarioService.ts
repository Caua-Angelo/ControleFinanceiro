import { api } from "./Apiservice";

export interface UsuarioRequest {
  nome: string;
  idade: number;
}

export async function criarUsuario(data: UsuarioRequest) {
  const response = await api.post("/usuarios", data);
  return response.data;
}

export async function consultarUsuario() {
  const response = await api.get("/usuarios");
  return response.data;
}

export async function deletarUsuario(id: number) {
  await api.delete(`/usuarios/${id}`);
}

export async function alterarUsuario(id: number, data: UsuarioRequest) {
  const response = await api.put(`/usuarios/${id}`, data);
  return response.data;
}
