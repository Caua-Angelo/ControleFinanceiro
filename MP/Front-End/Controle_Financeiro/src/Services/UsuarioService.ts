import { api } from "./Apiservice";

export interface CriarUsuarioRequest {
    nome: string;
    idade: number;
}
export async function criarUsuario(data: CriarUsuarioRequest) {
    const response = await api.post("/usuarios/criar", data);
    return response.data;
}