using CarCatalog.Models;
using CarCatalog.Models.Requests;
using CarCatalog.Models.Responses;
using CarCatalog.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CarCatalog.Services
{
    public class AuthService : IAuthService
    {
        private readonly CarCatalogDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(CarCatalogDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null || !VerifyPassword(request.Password, user.PasswordHash))
                return null;

            var token = GenerateJwtToken(user);

            return new AuthResponse
            {
                Token = token,
                User = new UserResponse
                {
                    Id = user.Id,
                    Email = user.Email,
                    Role = user.Role
                }
            };
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (existingUser != null)
                throw new Exception("Пользователь с таким email уже существует");

            if (request.Password != request.ConfirmPassword)
                throw new Exception("Пароли не совпадают");

            var user = new User
            {
                Email = request.Email,
                PasswordHash = HashPassword(request.Password),
                Role = "user",
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var token = GenerateJwtToken(user);

            return new AuthResponse
            {
                Token = token,
                User = new UserResponse
                {
                    Id = user.Id,
                    Email = user.Email,
                    Role = user.Role
                }
            };
        }

        public async Task<UserProfileResponse> GetUserProfileAsync(int userId)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return null;

            return new UserProfileResponse
            {
                Id = user.Id,
                Email = user.Email,
                Role = user.Role,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                AvatarUrl = user.AvatarUrl,
                Phone = user.Phone,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("userId", user.Id.ToString())
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(
                    Convert.ToDouble(_configuration["Jwt:ExpireHours"] ?? "3")),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, storedHash);
        }

        public async Task<UserProfileResponse> UpdateUserProfileAsync(int userId, UpdateProfileRequest request)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new Exception("Пользователь не найден");

            // Проверяем текущий пароль
            if (!VerifyPassword(request.CurrentPassword, user.PasswordHash))
                throw new Exception("Неверный текущий пароль");

            // Проверяем уникальность email
            if (user.Email != request.Email)
            {
                var emailExists = await _context.Users
                    .AnyAsync(u => u.Email == request.Email && u.Id != userId);

                if (emailExists)
                    throw new Exception("Пользователь с таким email уже существует");

                user.Email = request.Email;
            }

            // Проверяем уникальность username (если нужно)
            if (!string.IsNullOrEmpty(request.Username) && user.Username != request.Username)
            {
                var usernameExists = await _context.Users
                    .AnyAsync(u => u.Username == request.Username && u.Id != userId);

                if (usernameExists)
                    throw new Exception("Пользователь с таким именем уже существует");

                user.Username = request.Username;
            }

            // Обновляем остальные поля
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.AvatarUrl = request.AvatarUrl;
            user.Phone = request.Phone;
            user.UpdatedAt = DateTime.UtcNow;

            // Обновляем пароль, если указан новый
            if (!string.IsNullOrEmpty(request.NewPassword))
            {
                user.PasswordHash = HashPassword(request.NewPassword);
            }

            await _context.SaveChangesAsync();

            // Возвращаем обновленный профиль
            return new UserProfileResponse
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                AvatarUrl = user.AvatarUrl,
                Phone = user.Phone,
                Role = user.Role ?? "user",
                CreatedAt = user.CreatedAt ?? DateTime.UtcNow,
                UpdatedAt = user.UpdatedAt
            };
        }
    }
}