import api from './api';
import authService from './authService';

// Interfaces para os dados de currículo
export interface CreateCurriculumData {
  title: string;
  summary?: string;
  status: 'Draft' | 'Active' | 'Hidden' | 'Archived';
}

export interface CurriculumResponse {
  success: boolean;
  message: string;
  data: {
    id: string;
    userId: string;
    title: string;
    summary?: string;
    status: string;
    createdAt: string;
    updatedAt: string;
    userName?: string;
    shortLinks: Array<{
      id: string;
      curriculumId: string;
      hash: string;
      accessUrl: string;
      isRevoked: boolean;
      createdAt: string;
      revokedAt?: string;
    }>;
    experiences: Array<{
      id: string;
      curriculumId: string;
      companyName: string;
      role: string;
      startDate: string;
      endDate?: string;
      description?: string;
      location?: string;
    }>;
    educations: Array<{
      id: string;
      curriculumId: string;
      institution: string;
      degree: string;
      fieldOfStudy: string;
      startDate: string;
      endDate?: string;
      description?: string;
    }>;
    skills: Array<{
      id: string;
      curriculumId: string;
      techName: string;
      proficiency: string;
    }>;
    languages: Array<{
      id: string;
      curriculumId: string;
      languageName: string;
      proficiency: string;
    }>;
    contacts: Array<{
      id: string;
      curriculumId: string;
      type: string;
      value: string;
      isPrimary: boolean;
    }>;
    addresses: Array<{
      id: string;
      curriculumId: string;
      street: string;
      number: string;
      complement?: string;
      neighborhood: string;
      city: string;
      state: string;
      country?: string;
      zipCode?: string;
      type: string;
    }>;
  };
  errors?: string[];
}

export interface CurriculumListResponse {
  success: boolean;
  message: string;
  data: Array<{
    id: string;
    userId: string;
    title: string;
    summary?: string;
    status: string;
    createdAt: string;
    updatedAt: string;
    shortLinks: Array<{
      hash: string;
      isActive: boolean;
    }>;
  }>;
  errors?: string[];
}

// Serviço de currículos
const curriculumService = {
  // Criar um novo currículo
  async create(data: CreateCurriculumData): Promise<CurriculumResponse['data']> {
    try {
      const user = authService.getCurrentUser();
      if (!user) {
        throw new Error('Usuário não autenticado');
      }

      const response = await api.post<CurriculumResponse>(`/curriculums/user/${user.id}`, data);
      
      if (response.data.success) {
        return response.data.data;
      } else {
        throw new Error(response.data.message || 'Falha ao criar currículo');
      }
    } catch (error: any) {
      if (error.response && error.response.data) {
        throw new Error(error.response.data.message || 'Falha ao criar currículo');
      }
      throw error;
    }
  },

  // Obter todos os currículos do usuário atual
  async getUserCurriculums(): Promise<CurriculumListResponse['data']> {
    try {
      const user = authService.getCurrentUser();
      if (!user) {
        throw new Error('Usuário não autenticado');
      }

      const response = await api.get<CurriculumListResponse>(`/curriculums/user/${user.id}`);
      
      if (response.data.success) {
        return response.data.data;
      } else {
        throw new Error(response.data.message || 'Falha ao buscar currículos');
      }
    } catch (error: any) {
      if (error.response && error.response.data) {
        throw new Error(error.response.data.message || 'Falha ao buscar currículos');
      }
      throw error;
    }
  },

  // Obter um currículo específico
  async getById(id: string): Promise<CurriculumResponse['data']> {
    try {
      const response = await api.get<CurriculumResponse>(`/curriculums/${id}/complete`);

      if (response.data.success) {
        return response.data.data;
      } else {
        throw new Error(response.data.message || 'Falha ao buscar currículo');
      }
    } catch (error: any) {
      if (error.response && error.response.data) {
        throw new Error(error.response.data.message || 'Falha ao buscar currículo');
      }
      throw error;
    }
  },

  // Atualizar um currículo
  async update(id: string, data: Partial<CreateCurriculumData>): Promise<CurriculumResponse['data']> {
    try {
      const response = await api.put<CurriculumResponse>(`/curriculums/${id}`, data);
      
      if (response.data.success) {
        return response.data.data;
      } else {
        throw new Error(response.data.message || 'Falha ao atualizar currículo');
      }
    } catch (error: any) {
      if (error.response && error.response.data) {
        throw new Error(error.response.data.message || 'Falha ao atualizar currículo');
      }
      throw error;
    }
  },

  // Deletar um currículo
  async delete(id: string): Promise<void> {
    try {
      const response = await api.delete(`/curriculums/${id}`);

      if (!response.data.success) {
        throw new Error(response.data.message || 'Falha ao deletar currículo');
      }
    } catch (error: any) {
      if (error.response && error.response.data) {
        throw new Error(error.response.data.message || 'Falha ao deletar currículo');
      }
      throw error;
    }
  },

  // Obter um currículo através de link curto
  async getByShortLink(hash: string): Promise<CurriculumResponse['data']> {
    try {
      const response = await api.get<CurriculumResponse>(`/curriculums/shortlink/${hash}`);

      if (response.data.success) {
        return response.data.data;
      } else {
        throw new Error(response.data.message || 'Link curto não encontrado');
      }
    } catch (error: any) {
      if (error.response && error.response.data) {
        throw new Error(error.response.data.message || 'Link curto não encontrado');
      }
      throw error;
    }
  },

  // Alterar visibilidade do currículo
  async updateVisibility(id: string, isPublic: boolean): Promise<CurriculumResponse['data']> {
    try {
      const response = await api.patch<CurriculumResponse>(`/curriculums/${id}/visibility`, {
        isPublic
      });

      if (response.data.success) {
        return response.data.data;
      } else {
        throw new Error(response.data.message || 'Falha ao alterar visibilidade');
      }
    } catch (error: any) {
      if (error.response && error.response.data) {
        throw new Error(error.response.data.message || 'Falha ao alterar visibilidade');
      }
      throw error;
    }
  }
};

export default curriculumService;
