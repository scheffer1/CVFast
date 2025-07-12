using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using CVFastServices.DTOs;
using CVFastServices.Models;
using CVFastServices.Models.Auth;
using CVFastServices.Repositories.Interfaces;
using CVFastServices.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CVFastServices.Services
{
    /// <summary>
    /// Implementação do serviço de autenticação
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtSettings _jwtSettings;
        private readonly ILogger<AuthService> _logger;

        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="userRepository">Repositório de usuários</param>
        /// <param name="jwtSettings">Configurações do JWT</param>
        /// <param name="logger">Logger</param>
        public AuthService(
            IUserRepository userRepository,
            IOptions<JwtSettings> jwtSettings,
            ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _jwtSettings = jwtSettings.Value;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<AuthResponseDTO?> AuthenticateAsync(string email, string password)
        {
            try
            {
                var user = await _userRepository.GetByEmailAsync(email);
                if (user == null || !VerifyPassword(password, user.PasswordHash))
                {
                    return null;
                }

                var (token, expiration) = GenerateJwtToken(user);

                return new AuthResponseDTO
                {
                    Token = token,
                    Expiration = expiration,
                    User = new UserDTO
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Email = user.Email,
                        CreatedAt = user.CreatedAt
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao autenticar usuário: {Email}", email);
                return null;
            }
        }

        /// <inheritdoc/>
        public async Task<AuthResponseDTO?> RegisterAsync(RegisterDTO registerDto)
        {
            try
            {
                // Verificar se o email já está em uso
                if (await _userRepository.EmailExistsAsync(registerDto.Email))
                {
                    return null;
                }

                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Name = registerDto.Name,
                    Email = registerDto.Email,
                    PasswordHash = HashPassword(registerDto.Password),
                    CreatedAt = DateTime.UtcNow
                };

                await _userRepository.AddAsync(user);
                await _userRepository.SaveChangesAsync();

                // Gerar token JWT para o usuário recém-registrado
                var (token, expiration) = GenerateJwtToken(user);

                return new AuthResponseDTO
                {
                    Token = token,
                    Expiration = expiration,
                    User = new UserDTO
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Email = user.Email,
                        CreatedAt = user.CreatedAt
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao registrar usuário: {Email}", registerDto.Email);
                return null;
            }
        }

        /// <inheritdoc/>
        public (string token, DateTime expiration) GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email),
                new(JwtRegisteredClaimNames.Name, user.Name),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var expiration = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: expiration,
                signingCredentials: credentials
            );

            return (new JwtSecurityTokenHandler().WriteToken(token), expiration);
        }

        /// <inheritdoc/>
        public UserClaims? ValidateToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _jwtSettings.Audience,
                    ClockSkew = TimeSpan.Zero
                }, out var validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = Guid.Parse(jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value);
                var email = jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Email).Value;
                var name = jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Name).Value;

                return new UserClaims
                {
                    UserId = userId,
                    Email = email,
                    Name = name
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao validar token JWT");
                return null;
            }
        }

        /// <inheritdoc/>
        public string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        /// <inheritdoc/>
        public bool VerifyPassword(string password, string passwordHash)
        {
            return HashPassword(password) == passwordHash;
        }
    }
}
