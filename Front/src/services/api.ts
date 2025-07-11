import axios from 'axios';

// Função para obter a URL base da API dinamicamente
const getApiBaseUrl = () => {
  const hostname = window.location.hostname;
  const protocol = window.location.protocol;

  // Em desenvolvimento (localhost)
  if (hostname === 'localhost' || hostname === '127.0.0.1') {
    return `${protocol}//${hostname}:5207/api`;
  }

  // Em produção com domínio
  if (hostname === 'cvfast.com.br' || hostname === 'www.cvfast.com.br') {
    // Usar HTTPS se disponível, senão HTTP
    const apiProtocol = protocol === 'https:' ? 'https:' : 'http:';
    return `${apiProtocol}//web.cvfast.com.br/api`;
  }

  // Para outros casos (IP da VPS durante testes)
  return `${protocol}//web.${hostname}/api`;
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
    // Se o erro for 401 (Unauthorized), verificar se não é uma tentativa de login/registro
    if (error.response && error.response.status === 401) {
      const requestUrl = error.config?.url || '';

      // Não redirecionar se for uma tentativa de login ou registro
      if (!requestUrl.includes('/auth/login') && !requestUrl.includes('/auth/register')) {
        localStorage.removeItem('auth_token');
        localStorage.removeItem('current_user');
        window.location.href = '/login';
      }
    }
    return Promise.reject(error);
  }
);

export default api;
