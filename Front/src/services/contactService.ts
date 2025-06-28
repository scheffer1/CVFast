import api from './api';

// Interfaces para contatos
export interface CreateContactData {
  curriculumId: string;
  type: 'Email' | 'Phone' | 'LinkedIn' | 'GitHub' | 'Website' | 'WhatsApp';
  value: string;
  isPrimary: boolean;
}

export interface ContactResponse {
  success: boolean;
  message: string;
  data: {
    id: string;
    curriculumId: string;
    type: string;
    value: string;
    isPrimary: boolean;
  };
  errors?: string[];
}

// Serviço de contatos
const contactService = {
  // Criar um novo contato
  async create(data: CreateContactData): Promise<ContactResponse['data']> {
    try {
      const response = await api.post<ContactResponse>('/contacts', data);

      if (response.data.success) {
        return response.data.data;
      } else {
        throw new Error(response.data.message || 'Falha ao criar contato');
      }
    } catch (error: any) {
      if (error.response && error.response.data) {
        throw new Error(error.response.data.message || 'Falha ao criar contato');
      }
      throw error;
    }
  },

  // Atualizar um contato
  async update(id: string, data: Partial<CreateContactData>): Promise<ContactResponse['data']> {
    try {
      const response = await api.put<ContactResponse>(`/contacts/${id}`, data);
      
      if (response.data.success) {
        return response.data.data;
      } else {
        throw new Error(response.data.message || 'Falha ao atualizar contato');
      }
    } catch (error: any) {
      if (error.response && error.response.data) {
        throw new Error(error.response.data.message || 'Falha ao atualizar contato');
      }
      throw error;
    }
  },

  // Deletar um contato
  async delete(id: string): Promise<void> {
    try {
      const response = await api.delete(`/contacts/${id}`);
      
      if (!response.data.success) {
        throw new Error(response.data.message || 'Falha ao deletar contato');
      }
    } catch (error: any) {
      if (error.response && error.response.data) {
        throw new Error(error.response.data.message || 'Falha ao deletar contato');
      }
      throw error;
    }
  }
};

export default contactService;
