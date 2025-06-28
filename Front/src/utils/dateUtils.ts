/**
 * Converte uma string de data do frontend para o formato esperado pelo backend (DateOnly)
 * @param dateString - String de data no formato "YYYY-MM-DD" ou "YYYY"
 * @returns String de data no formato ISO (YYYY-MM-DD) ou null se vazia
 */
export function formatDateForBackend(dateString: string): string | null {
  if (!dateString || dateString.trim() === '') return null;

  // Se a string tem apenas 4 caracteres, assume que é apenas o ano
  if (dateString.length === 4) {
    return `${dateString}-01-01`;
  }

  // Se já está no formato correto (YYYY-MM-DD), retorna como está
  if (dateString.match(/^\d{4}-\d{2}-\d{2}$/)) {
    return dateString;
  }

  // Tenta converter outros formatos comuns
  try {
    const date = new Date(dateString);
    if (isNaN(date.getTime())) {
      throw new Error('Data inválida');
    }

    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');

    return `${year}-${month}-${day}`;
  } catch (error) {
    console.warn('Erro ao converter data:', dateString, error);
    return null; // Retorna null se não conseguir converter
  }
}

/**
 * Converte uma data do backend para exibição no frontend
 * @param dateString - String de data no formato ISO (YYYY-MM-DD)
 * @returns String formatada para exibição
 */
export function formatDateForDisplay(dateString: string): string {
  if (!dateString) return '';
  
  try {
    const date = new Date(dateString);
    if (isNaN(date.getTime())) {
      return dateString;
    }
    
    return date.toLocaleDateString('pt-BR');
  } catch (error) {
    console.warn('Erro ao formatar data para exibição:', dateString, error);
    return dateString;
  }
}
