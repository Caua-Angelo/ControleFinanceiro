import axios from 'axios';

export const api = axios.create({
  baseURL: 'https://localhost:7244/api', // URL da sua WebAPI
});

// Exemplo de função para login
export const login = async (email: string, senha: string) => {
  const response = await api.post('/auth/login', { email, senha });
  return response.data;
};