import api from './api';

// Interfaces para os dados de endereço
export interface CreateAddressData {
  curriculumId: string;
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

export interface AddressResponse {
  success: boolean;
  message: string;
  data: {
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
  };
  errors?: string[];
}

// Serviço de endereços
const addressService = {
  // Criar um novo endereço
  async create(data: CreateAddressData): Promise<AddressResponse['data']> {
    try {
      const response = await api.post<AddressResponse>('/addresses', data);
      
      if (response.data.success) {
        return response.data.data;
      } else {
        throw new Error(response.data.message || 'Falha ao criar endereço');
      }
    } catch (error: any) {
      if (error.response && error.response.data) {
        throw new Error(error.response.data.message || 'Falha ao criar endereço');
      }
      throw error;
    }
  },

  // Atualizar um endereço
  async update(id: string, data: Partial<CreateAddressData>): Promise<AddressResponse['data']> {
    try {
      const response = await api.put<AddressResponse>(`/addresses/${id}`, data);
      
      if (response.data.success) {
        return response.data.data;
      } else {
        throw new Error(response.data.message || 'Falha ao atualizar endereço');
      }
    } catch (error: any) {
      if (error.response && error.response.data) {
        throw new Error(error.response.data.message || 'Falha ao atualizar endereço');
      }
      throw error;
    }
  },

  // Deletar um endereço
  async delete(id: string): Promise<void> {
    try {
      const response = await api.delete(`/addresses/${id}`);
      
      if (!response.data.success) {
        throw new Error(response.data.message || 'Falha ao deletar endereço');
      }
    } catch (error: any) {
      if (error.response && error.response.data) {
        throw new Error(error.response.data.message || 'Falha ao deletar endereço');
      }
      throw error;
    }
  }
};

export default addressService;
