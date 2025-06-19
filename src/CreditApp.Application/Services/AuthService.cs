using System;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CreditApp.Application.DTOs;
using CreditApp.Domain.Entities;
using CreditApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using CreditApp.Domain.Enums;
using BCrypt.Net;

namespace CreditApp.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                throw new InvalidOperationException("El correo electrónico ya está registrado");

            if (await _context.Users.AnyAsync(u => u.Username == request.Username))
                throw new InvalidOperationException("El nombre de usuario ya está en uso");

            var user = new User(
                request.Username,
                request.Email,
                BCrypt.Net.BCrypt.HashPassword(request.Password),
                UserRole.User
            );

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var token = GenerateJwtToken(user);
            return new AuthResponse(token, user.Username, user.Email, user.Role.ToString());
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                throw new InvalidOperationException("Correo electrónico o contraseña inválidos");

            if (!user.IsActive)
                throw new InvalidOperationException("La cuenta está desactivada");

            user.UpdateLastLogin();
            await _context.SaveChangesAsync();

            var token = GenerateJwtToken(user);
            return new AuthResponse(token, user.Username, user.Email, user.Role.ToString());
        }

        public async Task<AuthResponse> RegisterAnalystAsync(RegisterRequest request)
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                throw new InvalidOperationException("El correo electrónico ya está registrado");

            if (await _context.Users.AnyAsync(u => u.Username == request.Username))
                throw new InvalidOperationException("El nombre de usuario ya está en uso");

            var user = new User(
                request.Username,
                request.Email,
                BCrypt.Net.BCrypt.HashPassword(request.Password),
                UserRole.Analyst
            );

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var token = GenerateJwtToken(user);
            return new AuthResponse(token, user.Username, user.Email, user.Role.ToString());
        }

        public async Task<bool> UpdatePasswordAsync(Guid userId, string currentPassword, string newPassword)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("Usuario no encontrado");

            if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
                throw new UnauthorizedAccessException("La contraseña actual es incorrecta");

            user.UpdatePassword(BCrypt.Net.BCrypt.HashPassword(newPassword));
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateEmailAsync(Guid userId, string newEmail)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("Usuario no encontrado");

            if (await _context.Users.AnyAsync(u => u.Email == newEmail && u.Id != userId))
                throw new ArgumentException("El correo electrónico ya está registrado");

            user.UpdateEmail(newEmail);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("Usuario no encontrado");

            if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
                throw new UnauthorizedAccessException("La contraseña actual es incorrecta");

            user.UpdatePassword(BCrypt.Net.BCrypt.HashPassword(newPassword));
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<AuthResponse> GetCurrentUserAsync(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("Usuario no encontrado");

            if (!user.IsActive)
                throw new InvalidOperationException("La cuenta está desactivada");

            var token = GenerateJwtToken(user);
            return new AuthResponse(token, user.Username, user.Email, user.Role.ToString());
        }

        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(Convert.ToDouble(_configuration["Jwt:ExpiryInHours"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
} 