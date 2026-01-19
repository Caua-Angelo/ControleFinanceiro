import type { Finalidade } from "../Types/Finalidade";
import { api } from "./Apiservice";

export interface CategoriaRequest {
  descricao: string;
  finalidade: number;
}

export interface CategoriaResponse {
  id: number;
  descricao: string;
  finalidade: Finalidade;
}

export async function criarCategoria(data: CategoriaRequest) {
  const response = await api.post("/categorias", data);
  return response.data;
}

export async function consultarCategoria() {
  const response = await api.get("/categorias");
  return response.data;
}

export async function deletarCategoria(id: number) {
  return api.delete(`/categorias/${id}`);
}

export async function alterarCategoria(id: number, data: CategoriaRequest) {
  const response = await api.put(`/categorias/${id}`, data);
  return response.data;
}
