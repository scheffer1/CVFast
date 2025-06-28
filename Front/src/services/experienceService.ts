import api from './api';

// Interfaces para experiências
export interface CreateExperienceData {
  curriculumId: string;
  companyName: string;
  role: string;
  startDate: string;
  endDate?: string;
  description?: string;
  location?: string;
}

export interface ExperienceResponse {
  success: boolean;
  message: string;
  data: {
    id: string;
    curriculumId: string;
    companyName: string;
    role: string;
    startDate: string;
    endDate?: string;
    description?: string;
    location?: string;
  };
  errors?: string[];
}

// Serviço de experiências
const experienceService = {
  // Criar uma nova experiência
  async create(data: CreateExperienceData): Promise<ExperienceResponse['data']> {
    try {
      const response = await api.post<ExperienceResponse>('/experiences', data);

      if (response.data.success) {
        return response.data.data;
      } else {
        throw new Error(response.data.message || 'Falha ao criar experiência');
      }
    } catch (error: any) {
      if (error.response && error.response.data) {
        throw new Error(error.response.data.message || 'Falha ao criar experiência');
      }
      throw error;
    }
  },

  // Atualizar uma experiência
  async update(id: string, data: Partial<CreateExperienceData>): Promise<ExperienceResponse['data']> {
    try {
      const response = await api.put<ExperienceResponse>(`/experiences/${id}`, data);
      
      if (response.data.success) {
        return response.data.data;
      } else {
        throw new Error(response.data.message || 'Falha ao atualizar experiência');
      }
    } catch (error: any) {
      if (error.response && error.response.data) {
        throw new Error(error.response.data.message || 'Falha ao atualizar experiência');
      }
      throw error;
    }
  },

  // Deletar uma experiência
  async delete(id: string): Promise<void> {
    try {
      const response = await api.delete(`/experiences/${id}`);
      
      if (!response.data.success) {
        throw new Error(response.data.message || 'Falha ao deletar experiência');
      }
    } catch (error: any) {
      if (error.response && error.response.data) {
        throw new Error(error.response.data.message || 'Falha ao deletar experiência');
      }
      throw error;
    }
  }
};

export default experienceService;
