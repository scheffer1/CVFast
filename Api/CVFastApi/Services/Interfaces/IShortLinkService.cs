using CVFastApi.Models;

namespace CVFastApi.Services.Interfaces
{
    /// <summary>
    /// Interface para o serviço de links curtos
    /// </summary>
    public interface IShortLinkService
    {
        /// <summary>
        /// Gera um novo link curto para um currículo
        /// </summary>
        /// <param name="curriculumId">ID do currículo</param>
        /// <returns>Link curto gerado</returns>
        Task<ShortLink> GenerateShortLinkAsync(Guid curriculumId);
        
        /// <summary>
        /// Obtém um currículo pelo hash do link curto
        /// </summary>
        /// <param name="hash">Hash do link curto</param>
        /// <returns>Currículo associado ao link curto</returns>
        Task<Curriculum?> GetCurriculumByHashAsync(string hash);
        
        /// <summary>
        /// Registra um acesso a um link curto
        /// </summary>
        /// <param name="shortLinkId">ID do link curto</param>
        /// <param name="ip">Endereço IP do visitante</param>
        /// <param name="userAgent">User Agent do navegador ou aplicativo</param>
        /// <returns>Log de acesso registrado</returns>
        Task<AccessLog> RegisterAccessAsync(Guid shortLinkId, string ip, string? userAgent);
    }
}
