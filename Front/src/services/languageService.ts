import api from './api';

// Interfaces para os dados de idioma
export interface CreateLanguageData {
  curriculumId: string;
  languageName: string;
  proficiency: 'Beginner' | 'Intermediate' | 'Advanced' | 'Fluent' | 'Native';
}

export interface LanguageResponse {
  success: boolean;
  message: string;
  data: {
    id: string;
    curriculumId: string;
    languageName: string;
    proficiency: string;
  };
  errors?: string[];
}

// Serviço de idiomas
const languageService = {
  // Criar um novo idioma
  async create(data: CreateLanguageData): Promise<LanguageResponse['data']> {
    try {
      const response = await api.post<LanguageResponse>('/languages', data);
      
      if (response.data.success) {
        return response.data.data;
      } else {
        throw new Error(response.data.message || 'Falha ao criar idioma');
      }
    } catch (error: any) {
      if (error.response && error.response.data) {
        throw new Error(error.response.data.message || 'Falha ao criar idioma');
      }
      throw error;
    }
  },

  // Atualizar um idioma
  async update(id: string, data: Partial<CreateLanguageData>): Promise<LanguageResponse['data']> {
    try {
      const response = await api.put<LanguageResponse>(`/languages/${id}`, data);
      
      if (response.data.success) {
        return response.data.data;
      } else {
        throw new Error(response.data.message || 'Falha ao atualizar idioma');
      }
    } catch (error: any) {
      if (error.response && error.response.data) {
        throw new Error(error.response.data.message || 'Falha ao atualizar idioma');
      }
      throw error;
    }
  },

  // Deletar um idioma
  async delete(id: string): Promise<void> {
    try {
      const response = await api.delete(`/languages/${id}`);
      
      if (!response.data.success) {
        throw new Error(response.data.message || 'Falha ao deletar idioma');
      }
    } catch (error: any) {
      if (error.response && error.response.data) {
        throw new Error(error.response.data.message || 'Falha ao deletar idioma');
      }
      throw error;
    }
  }
};

export default languageService;
