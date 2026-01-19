import type { TransacaoResponse } from "../Types/TransacaoResponse";
import { api } from "./Apiservice";

export interface TransacaoRequest {
  descricao: string;
  valor: number;
  tipo: number;
  categoriaId: number;
  usuarioId: number;
  data : string;
}

export async function listarTransacoes(params?: {
  usuarioId?: number;
  categoriaId?: number;
}) {
  const response = await api.get("/transacoes", { params });
  return response.data as TransacaoResponse[];
}

export async function consultarTransacaoPorId(id: number) {
  const response = await api.get(`/transacoes/${id}`);
  return response.data as TransacaoResponse;
}

export async function criarTransacao(data: TransacaoRequest) {
  const response = await api.post("/transacoes", data);
  return response.data as TransacaoResponse;
}

export async function alterarTransacao(id: number, data: TransacaoRequest) {
  const response = await api.put(`/transacoes/${id}`, data);
  return response.data as TransacaoResponse;
}

export async function deletarTransacao(id: number) {
  await api.delete(`/transacoes/${id}`);
}
