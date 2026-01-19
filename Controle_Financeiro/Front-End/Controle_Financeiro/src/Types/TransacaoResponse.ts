export interface TransacaoResponse {
  id: number;
  descricao: string;
  valor: number;
  tipo: number;
  usuarioId: number;
  usuarioNome: string;
  categoriaId: number;
  categoriaDescricao: string;
  data: string;
}
