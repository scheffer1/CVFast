import api from './api';

// Interfaces para habilidades
export interface CreateSkillData {
  curriculumId: string;
  techName: string;
  proficiency: 'Basic' | 'Intermediate' | 'Advanced' | 'Expert';
}

export interface SkillResponse {
  success: boolean;
  message: string;
  data: {
    id: string;
    curriculumId: string;
    techName: string;
    proficiency: string;
  };
  errors?: string[];
}

// Serviço de habilidades
const skillService = {
  // Criar uma nova habilidade
  async create(data: CreateSkillData): Promise<SkillResponse['data']> {
    try {
      const response = await api.post<SkillResponse>('/skills', data);

      if (response.data.success) {
        return response.data.data;
      } else {
        throw new Error(response.data.message || 'Falha ao criar habilidade');
      }
    } catch (error: any) {
      if (error.response && error.response.data) {
        throw new Error(error.response.data.message || 'Falha ao criar habilidade');
      }
      throw error;
    }
  },

  // Atualizar uma habilidade
  async update(id: string, data: Partial<CreateSkillData>): Promise<SkillResponse['data']> {
    try {
      const response = await api.put<SkillResponse>(`/skills/${id}`, data);
      
      if (response.data.success) {
        return response.data.data;
      } else {
        throw new Error(response.data.message || 'Falha ao atualizar habilidade');
      }
    } catch (error: any) {
      if (error.response && error.response.data) {
        throw new Error(error.response.data.message || 'Falha ao atualizar habilidade');
      }
      throw error;
    }
  },

  // Deletar uma habilidade
  async delete(id: string): Promise<void> {
    try {
      const response = await api.delete(`/skills/${id}`);
      
      if (!response.data.success) {
        throw new Error(response.data.message || 'Falha ao deletar habilidade');
      }
    } catch (error: any) {
      if (error.response && error.response.data) {
        throw new Error(error.response.data.message || 'Falha ao deletar habilidade');
      }
      throw error;
    }
  }
};

export default skillService;
