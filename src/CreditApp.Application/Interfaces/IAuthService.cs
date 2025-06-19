using System;
using System.Threading.Tasks;
using CreditApp.Application.DTOs;

namespace CreditApp.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<bool> DeactivateAccountAsync(Guid userId);
        Task<AuthResponse> GetCurrentUserAsync(Guid userId);
    }
} 