using System.Text;
using CVFastApi.Models;
using CVFastApi.Repositories.Interfaces;
using CVFastApi.Services.Interfaces;

namespace CVFastApi.Services
{
    /// <summary>
    /// Implementação do serviço de links curtos
    /// </summary>
    public class ShortLinkService : IShortLinkService
    {
        private readonly IShortLinkRepository _shortLinkRepository;
        private readonly ICurriculumRepository _curriculumRepository;
        private readonly IRepository<AccessLog> _accessLogRepository;
        private readonly ILogger<ShortLinkService> _logger;

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="shortLinkRepository">Repositório de links curtos</param>
        /// <param name="curriculumRepository">Repositório de currículos</param>
        /// <param name="accessLogRepository">Repositório de logs de acesso</param>
        /// <param name="logger">Logger</param>
        public ShortLinkService(
            IShortLinkRepository shortLinkRepository,
            ICurriculumRepository curriculumRepository,
            IRepository<AccessLog> accessLogRepository,
            ILogger<ShortLinkService> logger)
        {
            _shortLinkRepository = shortLinkRepository;
            _curriculumRepository = curriculumRepository;
            _accessLogRepository = accessLogRepository;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<ShortLink> GenerateShortLinkAsync(Guid curriculumId)
        {
            var curriculum = await _curriculumRepository.GetByIdAsync(curriculumId);
            if (curriculum == null)
            {
                throw new ArgumentException($"Currículo com ID {curriculumId} não encontrado");
            }

            var shortLinkId = Guid.NewGuid();
            
            var hash = Convert.ToBase64String(shortLinkId.ToByteArray())
                .Replace("/", "_")
                .Replace("+", "-")
                .Replace("=", "");
            
            hash = hash.Substring(0, Math.Min(hash.Length, 8));

            var shortLink = new ShortLink
            {
                Id = shortLinkId,
                CurriculumId = curriculumId,
                Hash = hash,
                IsRevoked = false,
                CreatedAt = DateTime.UtcNow
            };

            await _shortLinkRepository.AddAsync(shortLink);
            await _shortLinkRepository.SaveChangesAsync();

            _logger.LogInformation("Link curto gerado: {ShortLinkId} para o currículo: {CurriculumId}", 
                shortLink.Id, shortLink.CurriculumId);

            return shortLink;
        }

        /// <inheritdoc/>
        public async Task<Curriculum?> GetCurriculumByHashAsync(string hash)
        {
            var shortLink = await _shortLinkRepository.GetActiveByHashAsync(hash);
            if (shortLink == null)
            {
                return null;
            }

            return await _curriculumRepository.GetCompleteByIdAsync(shortLink.CurriculumId);
        }

        /// <inheritdoc/>
        public async Task<AccessLog> RegisterAccessAsync(Guid shortLinkId, string ip, string? userAgent)
        {
            var accessLog = new AccessLog
            {
                Id = Guid.NewGuid(),
                ShortLinkId = shortLinkId,
                IP = ip,
                UserAgent = userAgent,
                AccessedAt = DateTime.UtcNow
            };

            await _accessLogRepository.AddAsync(accessLog);
            await _accessLogRepository.SaveChangesAsync();

            _logger.LogInformation("Acesso registrado: {AccessLogId} para o link curto: {ShortLinkId}", 
                accessLog.Id, accessLog.ShortLinkId);

            return accessLog;
        }
    }
}
