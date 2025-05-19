import api from './api';
import { User } from '@/types';

// Interfaces para os dados de autenticação
export interface LoginCredentials {
  email: string;
  password: string;
}

export interface RegisterData {
  name: string;
  email: string;
  password: string;
  confirmPassword: string;
}

export interface AuthResponse {
  success: boolean;
  message: string;
  data: {
    token: string;
    expiration: string;
    user: User;
  };
  errors?: string[];
}

// Serviço de autenticação
const authService = {
  // Login de usuário
  async login(credentials: LoginCredentials): Promise<User> {
    try {
      const response = await api.post<AuthResponse>('/auth/login', credentials);
      
      if (response.data.success) {
        // Salvar o token e o usuário no localStorage
        localStorage.setItem('auth_token', response.data.data.token);
        localStorage.setItem('current_user', JSON.stringify(response.data.data.user));
        return response.data.data.user;
      } else {
        throw new Error(response.data.message || 'Falha na autenticação');
      }
    } catch (error: any) {
      if (error.response && error.response.data) {
        throw new Error(error.response.data.message || 'Credenciais inválidas');
      }
      throw error;
    }
  },

  // Registro de novo usuário
  async register(data: RegisterData): Promise<User> {
    try {
      const response = await api.post<AuthResponse>('/auth/register', data);
      
      if (response.data.success) {
        // Salvar o token e o usuário no localStorage
        localStorage.setItem('auth_token', response.data.data.token);
        localStorage.setItem('current_user', JSON.stringify(response.data.data.user));
        return response.data.data.user;
      } else {
        throw new Error(response.data.message || 'Falha no registro');
      }
    } catch (error: any) {
      if (error.response && error.response.data) {
        throw new Error(error.response.data.message || 'Falha no registro');
      }
      throw error;
    }
  },

  // Logout de usuário
  logout(): void {
    localStorage.removeItem('auth_token');
    localStorage.removeItem('current_user');
  },

  // Verificar se o usuário está autenticado
  isAuthenticated(): boolean {
    return !!localStorage.getItem('auth_token');
  },

  // Obter o usuário atual
  getCurrentUser(): User | null {
    const user = localStorage.getItem('current_user');
    return user ? JSON.parse(user) : null;
  }
};

export default authService;
