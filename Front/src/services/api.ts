import axios from 'axios';

// Função para obter a URL base da API dinamicamente
// Detecta automaticamente o hostname atual (localhost, IP, DNS, etc.)
// e aponta para o backend na porta 5207
const getApiBaseUrl = () => {
  const hostname = window.location.hostname;
  const port = '5207';
  const protocol = window.location.protocol;
  return `${protocol}//${hostname}:${port}/api`;
};

// Criando uma instância do axios com configurações padrão
const api = axios.create({
  baseURL: getApiBaseUrl(),
  headers: {
    'Content-Type': 'application/json',
  },
});

// Interceptor para adicionar o token de autenticação em todas as requisições
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('auth_token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Interceptor para tratar erros de resposta
api.interceptors.response.use(
  (response) => {
    return response;
  },
  (error) => {
    // Se o erro for 401 (Unauthorized), redirecionar para a página de login
    if (error.response && error.response.status === 401) {
      localStorage.removeItem('auth_token');
      localStorage.removeItem('current_user');
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

export default api;
