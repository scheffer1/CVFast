namespace CVFastServices.DTOs
{
    /// <summary>
    /// DTO para resposta padrão da API
    /// </summary>
    public class ApiResponse<T>
    {
        /// <summary>
        /// Indica se a operação foi bem-sucedida
        /// </summary>
        public bool Success { get; set; }
        
        /// <summary>
        /// Mensagem de retorno
        /// </summary>
        public string? Message { get; set; }
        
        /// <summary>
        /// Dados retornados pela operação
        /// </summary>
        public T? Data { get; set; }
        
        /// <summary>
        /// Erros ocorridos durante a operação
        /// </summary>
        public List<string>? Errors { get; set; }
        
        /// <summary>
        /// Cria uma resposta de sucesso
        /// </summary>
        /// <param name="data">Dados a serem retornados</param>
        /// <param name="message">Mensagem de sucesso</param>
        /// <returns>Resposta de sucesso</returns>
        public static ApiResponse<T> SuccessResponse(T data, string message = "Operação realizada com sucesso")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data
            };
        }
        
        /// <summary>
        /// Cria uma resposta de erro
        /// </summary>
        /// <param name="message">Mensagem de erro</param>
        /// <param name="errors">Lista de erros</param>
        /// <returns>Resposta de erro</returns>
        public static ApiResponse<T> ErrorResponse(string message, List<string>? errors = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Errors = errors
            };
        }
    }
}
