using System.Threading.Tasks;
using CreditApp.Application.DTOs;

namespace CreditApp.Application.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<AuthResponse> RegisterAnalystAsync(RegisterRequest request);
        Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword);
        Task<AuthResponse> GetCurrentUserAsync(Guid userId);
    }
} 