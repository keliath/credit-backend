using System.Threading;
using System.Threading.Tasks;
using MediatR;
using CreditApp.Application.DTOs;
using CreditApp.Application.Services;

namespace CreditApp.Application.Handlers
{
    public class RegisterHandler : IRequestHandler<RegisterCommand, AuthResponse>
    {
        private readonly IAuthService _authService;

        public RegisterHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            // Reutiliza la lógica existente
            return await _authService.RegisterAsync(new RegisterRequest(request.Username, request.Email, request.Password));
        }
    }
} 