import { api } from "./Apiservice";

export interface UsuarioRequest {
    nome: string;
    idade: number;
}
export async function criarUsuario(data: UsuarioRequest) {
    const response = await api.post("/usuarios/criar", data);
    return response.data;
}
export async function ListarUsuario() {
    const response = await api.get("/usuarios/consultar",);
    return response.data;
}