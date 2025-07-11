import api from './api';
import authService from './authService';

// Interfaces para currículo completo
export interface CreateCompleteCurriculumData {
  title: string;
  summary?: string;
  status: 'Draft' | 'Active' | 'Hidden' | 'Archived';
  experiences: CreateExperienceForCurriculumData[];
  educations: CreateEducationForCurriculumData[];
  skills: CreateSkillForCurriculumData[];
  languages: CreateLanguageForCurriculumData[];
  contacts: CreateContactForCurriculumData[];
  addresses: CreateAddressForCurriculumData[];
}

export interface CreateExperienceForCurriculumData {
  companyName: string;
  role: string;
  description?: string;
  startDate: string; // formato YYYY-MM-DD
  endDate?: string | null; // formato YYYY-MM-DD ou null
  location?: string;
}

export interface CreateEducationForCurriculumData {
  institution: string;
  degree: string;
  fieldOfStudy: string;
  startDate: string; // formato YYYY-MM-DD
  endDate?: string | null; // formato YYYY-MM-DD ou null
  description?: string;
}

export interface CreateSkillForCurriculumData {
  techName: string;
  proficiency: 'Basic' | 'Intermediate' | 'Advanced' | 'Expert';
}

export interface CreateLanguageForCurriculumData {
  languageName: string;
  proficiency: 'Beginner' | 'Intermediate' | 'Advanced' | 'Fluent' | 'Native';
}

export interface CreateContactForCurriculumData {
  type: 'Email' | 'Phone' | 'LinkedIn' | 'GitHub' | 'Website' | 'WhatsApp';
  value: string;
  isPrimary: boolean;
}

export interface CreateAddressForCurriculumData {
  street: string;
  number: string;
  complement?: string;
  neighborhood: string;
  city: string;
  state: string;
  country?: string;
  zipCode?: string;
  type: 'Residential' | 'Current' | 'Other';
}

export interface CompleteCurriculumResponse {
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
    experiences: any[];
    educations: any[];
    skills: any[];
    contacts: any[];
    addresses: any[];
    shortLinks: any[];
  };
  errors?: string[];
}

// Serviço de currículo completo
const completeCurriculumService = {
  // Criar um currículo completo
  async create(data: CreateCompleteCurriculumData): Promise<CompleteCurriculumResponse['data']> {
    try {
      const user = authService.getCurrentUser();
      if (!user) {
        throw new Error('Usuário não autenticado');
      }

      const response = await api.post<CompleteCurriculumResponse>(`/curriculums/user/${user.id}/complete`, data);
      
      if (response.data.success) {
        return response.data.data;
      } else {
        throw new Error(response.data.message || 'Falha ao criar currículo completo');
      }
    } catch (error: any) {
      if (error.response && error.response.data) {
        // Se há erros de validação específicos, incluí-los na mensagem
        if (error.response.data.errors && Array.isArray(error.response.data.errors)) {
          throw new Error(error.response.data.errors.join(', '));
        }
        throw new Error(error.response.data.message || 'Falha ao criar currículo completo');
      }
      throw error;
    }
  }
};

export default completeCurriculumService;
