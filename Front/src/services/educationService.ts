import api from './api';

// Interfaces para educação
export interface CreateEducationData {
  curriculumId: string;
  institution: string;
  degree: string;
  fieldOfStudy: string;
  startDate: string;
  endDate?: string;
}

export interface EducationResponse {
  success: boolean;
  message: string;
  data: {
    id: string;
    curriculumId: string;
    institution: string;
    degree: string;
    fieldOfStudy: string;
    startDate: string;
    endDate?: string;
  };
  errors?: string[];
}

// Serviço de educação
const educationService = {
  // Criar uma nova educação
  async create(data: CreateEducationData): Promise<EducationResponse['data']> {
    try {
      const response = await api.post<EducationResponse>('/educations', data);

      if (response.data.success) {
        return response.data.data;
      } else {
        throw new Error(response.data.message || 'Falha ao criar educação');
      }
    } catch (error: any) {
      if (error.response && error.response.data) {
        throw new Error(error.response.data.message || 'Falha ao criar educação');
      }
      throw error;
    }
  },

  // Atualizar uma educação
  async update(id: string, data: Partial<CreateEducationData>): Promise<EducationResponse['data']> {
    try {
      const response = await api.put<EducationResponse>(`/educations/${id}`, data);
      
      if (response.data.success) {
        return response.data.data;
      } else {
        throw new Error(response.data.message || 'Falha ao atualizar educação');
      }
    } catch (error: any) {
      if (error.response && error.response.data) {
        throw new Error(error.response.data.message || 'Falha ao atualizar educação');
      }
      throw error;
    }
  },

  // Deletar uma educação
  async delete(id: string): Promise<void> {
    try {
      const response = await api.delete(`/educations/${id}`);
      
      if (!response.data.success) {
        throw new Error(response.data.message || 'Falha ao deletar educação');
      }
    } catch (error: any) {
      if (error.response && error.response.data) {
        throw new Error(error.response.data.message || 'Falha ao deletar educação');
      }
      throw error;
    }
  }
};

export default educationService;
