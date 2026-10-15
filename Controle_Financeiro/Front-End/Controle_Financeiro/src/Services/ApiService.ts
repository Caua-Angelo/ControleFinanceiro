import axios, { AxiosError } from "axios";

export const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL,
});

let redirecionandoParaLogin = false;

function tokenExpirado(token: string): boolean {
  try {
    const partes = token.split(".");

    if (partes.length !== 3) {
      return true;
    }

    // Converte Base64URL para Base64 antes de decodificar.
    const payloadBase64 = partes[1].replace(/-/g, "+").replace(/_/g, "/");

    const payloadDecodificado = atob(payloadBase64.padEnd(payloadBase64.length + ((4 - (payloadBase64.length % 4)) % 4), "="));

    const payload = JSON.parse(payloadDecodificado);

    if (typeof payload.exp !== "number") {
      return true;
    }

    // Margem de 30 segundos para evitar requisições no limite da expiração.
    return payload.exp * 1000 <= Date.now() + 30_000;
  } catch {
    return true;
  }
}

function limparSessao(): void {
  localStorage.removeItem("token");
}

function redirecionarParaLogin(): void {
  limparSessao();

  if (window.location.pathname !== "/login" && !redirecionandoParaLogin) {
    redirecionandoParaLogin = true;
    window.location.href = "/login";
  }
}

function ehRequisicaoDeLogin(url?: string): boolean {
  if (!url) return false;

  try {
    const urlCompleta = new URL(url, api.defaults.baseURL || window.location.origin);
    return urlCompleta.pathname.replace(/\/+$/, "").endsWith("/auth/login");
  } catch {
    return url.includes("/auth/login");
  }
}

api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem("token");

    if (!token) {
      return config;
    }

    if (tokenExpirado(token)) {
      redirecionarParaLogin();

      return Promise.reject(new axios.CanceledError("Token expirado"));
    }

    config.headers.Authorization = `Bearer ${token}`;

    return config;
  },
  (error) => Promise.reject(error),
);

api.interceptors.response.use(
  (response) => response,
  (error: AxiosError) => {
    if (error.response?.status === 401 && !ehRequisicaoDeLogin(error.config?.url)) {
      redirecionarParaLogin();
    }

    return Promise.reject(error);
  },
);
