export interface TransacaoResponse {
  id: number;
  descricao: string;
  valor: number;
  tipo: number;
  usuarioId: number;
  usuarioNome: string;
  categoriaId: number;
  categoriaDescricao: string;
  finalidadeCategoria: number;
  data: string;
}
