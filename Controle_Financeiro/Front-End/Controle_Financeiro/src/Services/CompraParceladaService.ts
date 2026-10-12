import { api } from "./ApiService";
import type { CompraParceladaResponse } from "../Types/CompraParceladaResponse";

export interface CompraParceladaRequest {
  descricao: string;
  valorTotal: number;
  quantidadeParcelas: number;
  dataCompra: string;
  cartaoId: number;
}

export type CompraParceladaAtualizarRequest = CompraParceladaRequest;

export async function listarComprasParceladas() {
  const response = await api.get("/CompraParcelada");
  return response.data as CompraParceladaResponse[];
}

export async function consultarCompraParceladaPorId(id: number) {
  const response = await api.get(`/CompraParcelada/${id}`);
  return response.data as CompraParceladaResponse;
}

export async function criarCompraParcelada(data: CompraParceladaRequest) {
  const response = await api.post("/CompraParcelada", data);
  return response.data as CompraParceladaResponse;
}

export async function atualizarCompraParcelada(id: number, data: CompraParceladaAtualizarRequest) {
  const response = await api.put(`/CompraParcelada/${id}`, data);
  return response.data as CompraParceladaResponse;
}

export async function deletarCompraParcelada(id: number) {
  await api.delete(`/CompraParcelada/${id}`);
}

export async function marcarParcelaComoPaga(parcelaId: number) {
  await api.patch(`/CompraParcelada/parcelas/${parcelaId}/pagar`);
}

export async function reabrirParcela(parcelaId: number) {
  await api.patch(`/CompraParcelada/parcelas/${parcelaId}/reabrir`);
}
