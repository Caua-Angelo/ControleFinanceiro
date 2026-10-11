
import { api } from "./ApiService";

export interface CartaoRequest {
  nome: string;
  imagem: string;
  diaFechamento: number;
  diaVencimento: number;
}

export interface CartaoResponse {
  id: number;
  nome: string;
  imagem: string;
  diaFechamento: number;
  diaVencimento: number;
}

export async function criarCartao(data: CartaoRequest) {
  const response = await api.post("/Cartao", data);
  return response.data;
}

export async function consultarCartoes(): Promise<CartaoResponse[]> {
  const response = await api.get("/Cartao");
  return response.data;
}

export async function consultarCartao(id: number): Promise<CartaoResponse> {
  const response = await api.get(`/Cartao/${id}`);
  return response.data;
}

export async function alterarCartao(
  id: number,
  data: CartaoRequest
) {
  const response = await api.put(`/Cartao/${id}`, data);
  return response.data;
}

export async function deletarCartao(id: number) {
  return api.delete(`/Cartao/${id}`);
}
