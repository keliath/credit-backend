using System.Threading;
using System.Threading.Tasks;
using MediatR;
using CreditApp.Application.DTOs;
using CreditApp.Application.Services;

namespace CreditApp.Application.Handlers
{
    public class LoginHandler : IRequestHandler<LoginCommand, AuthResponse>
    {
        private readonly IAuthService _authService;

        public LoginHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            // Reutiliza la lógica existente
            return await _authService.LoginAsync(new LoginRequest(request.Email, request.Password));
        }
    }
} 