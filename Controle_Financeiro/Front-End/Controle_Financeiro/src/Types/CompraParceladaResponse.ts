export interface ParcelaResponse {
  id: number;
  numero: number;
  valor: number;
  dataVencimento: string;
  paga: boolean;
}

export interface CompraParceladaResponse {
  id: number;
  descricao: string;
  valorTotal: number;
  quantidadeParcelas: number;
  dataCompra: string;
  cartaoId: number;
  parcelas: ParcelaResponse[];
}
