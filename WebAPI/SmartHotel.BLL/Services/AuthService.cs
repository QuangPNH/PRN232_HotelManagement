using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SmartHotel.BLL.DTOs.Auth;
using SmartHotel.BLL.Services.Interface;
using SmartHotel.DAL.Models;
using SmartHotel.DAL.Repositories.Interface;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SmartHotel.BLL.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAccountRepository _accountRepo;
        private readonly IConfiguration _configuration;

        public AuthService(IAccountRepository accountRepo, IConfiguration configuration)
        {
            _accountRepo = accountRepo;
            _configuration = configuration;
        }

        public async Task<bool> RegisterAsync(RegisterRequest request)
        {
            if (await _accountRepo.IsEmailExistsAsync(request.Email))
            {
                throw new Exception("Email này đã được sử dụng.");
            }

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var newAccount = new Account
            {
                Email = request.Email, 
                PasswordHash = passwordHash,
                IsActive = false,
                CreatedAt = DateTime.Now,

                Tenant = new Tenant
                {
                    FullName = request.FullName,
                    Phone = request.Phone,
                    CCCD = request.CCCD,

                },

                AccountRoles = new List<AccountRole> { new AccountRole { RoleId = 3 } }
            };

            await _accountRepo.CreateAccountAsync(newAccount);
            return true;
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var account = await _accountRepo.GetAccountByEmailnameAsync(request.Email);

            if (account == null || !BCrypt.Net.BCrypt.Verify(request.Password, account.PasswordHash))
            {
                throw new Exception("Email hoặc mật khẩu không chính xác.");
            }

            if (!account.IsActive) throw new Exception("Tài khoản đã bị khóa.");

            var accessToken = GenerateAccessToken(account);
            var refreshToken = GenerateRefreshToken();

            var refreshTokenEntity = new RefreshToken
            {
                AccountId = account.AccountId,
                Token = refreshToken,
                IsRevoked = false,
                ExpiredAt = DateTime.Now.AddDays(1)
            };

            await _accountRepo.SaveRefreshTokenAsync(refreshTokenEntity);

            await _accountRepo.RemoveOldRefreshTokensAsync(account.AccountId);

            var roleName = account.AccountRoles.FirstOrDefault()?.Role.RoleName ?? "Unknown";
            var fullName = account.Tenant?.FullName ?? account.User?.FullName ?? "User";

            return new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.Now.AddMinutes(15),
                FullName = fullName,
                Role = roleName
            };
        }

        public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var secretKeyBytes = Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]);

            var tokenValidationParams = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false, 
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                var principal = jwtTokenHandler.ValidateToken(request.AccessToken, tokenValidationParams, out var validatedToken);

                if (validatedToken is JwtSecurityToken jwtSecurityToken &&
                    !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new Exception("Invalid Token Algo");
                }

                var storedRefreshToken = await _accountRepo.GetRefreshTokenAsync(request.RefreshToken);

                if (storedRefreshToken == null) throw new Exception("Refresh token không tồn tại.");
                if (storedRefreshToken.IsRevoked) throw new Exception("Refresh token đã bị thu hồi.");
                if (storedRefreshToken.ExpiredAt < DateTime.Now) throw new Exception("Refresh token đã hết hạn.");

                storedRefreshToken.IsRevoked = true;
                await _accountRepo.UpdateRefreshTokenAsync(storedRefreshToken);

                var account = await _accountRepo.GetAccountByIdAsync(storedRefreshToken.AccountId);
                var newAccessToken = GenerateAccessToken(account);
                var newRefreshToken = GenerateRefreshToken();

                var newRefreshTokenEntity = new RefreshToken
                {
                    AccountId = account.AccountId,
                    Token = newRefreshToken,
                    IsRevoked = false,
                    ExpiredAt = DateTime.Now.AddDays(30)
                };

                await _accountRepo.SaveRefreshTokenAsync(newRefreshTokenEntity);

                var roleName = account.AccountRoles?.FirstOrDefault()?.Role?.RoleName ?? "User";

                return new AuthResponse
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken,
                    ExpiresAt = DateTime.Now.AddMinutes(15),
                    FullName = account.Tenant?.FullName ?? account.User?.FullName ?? "Unknown",
                    Role = roleName
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi làm mới token: " + ex.Message);
            }
        }

        public async Task LogoutAsync(string refreshToken)
        {
            var storedToken = await _accountRepo.GetRefreshTokenAsync(refreshToken);
            if (storedToken != null)
            {
                storedToken.IsRevoked = true;
                await _accountRepo.UpdateRefreshTokenAsync(storedToken);
            }
        }

        public async Task<bool> ChangePasswordAsync(int accountId, string oldPass, string newPass)
        {
            var account = await _accountRepo.GetAccountByIdAsync(accountId);
            if (account == null) throw new Exception("Tài khoản không tồn tại.");

            if (!BCrypt.Net.BCrypt.Verify(oldPass, account.PasswordHash))
            {
                throw new Exception("Mật khẩu cũ không đúng.");
            }

            string newHash = BCrypt.Net.BCrypt.HashPassword(newPass);
            await _accountRepo.UpdatePasswordAsync(accountId, newHash);
            return true;
        }

        private string GenerateAccessToken(Account account)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var secretKeyBytes = Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]);
            var roles = account.AccountRoles?.Select(ar => ar.Role.RoleName).ToList() ?? new List<string>();

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, account.Email),
                new Claim(JwtRegisteredClaimNames.Email, account.Email),
                new Claim("AccountId", account.AccountId.ToString()),
                new Claim("FullName", account.Tenant?.FullName ?? account.User?.FullName ?? "")
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKeyBytes), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescription);
            return jwtTokenHandler.WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var random = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(random);
                return Convert.ToBase64String(random);
            }
        }
    }
}