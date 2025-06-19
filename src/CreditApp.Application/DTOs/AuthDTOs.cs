using System;
using MediatR;

namespace CreditApp.Application.DTOs
{
    public record RegisterRequest(
        string Username,
        string Email,
        string Password
    );

    public record LoginRequest(
        string Email,
        string Password
    );

    public record AuthResponse(
        string Token,
        string Username,
        string Email,
        string Role
    );

    // MediatR Commands auditables
    public record LoginCommand(
        string Email,
        string Password,
        string? IpAddress
    ) : IRequest<AuthResponse>, IAuditable
    {
        public string EntityName => "User";
        public Guid EntityId => Guid.Empty;
        public string Action => "Login";
        public string Details => $"User login from IP {IpAddress}";
        public string PerformedBy => Email;
    }

    public record RegisterCommand(
        string Username,
        string Email,
        string Password,
        string? IpAddress
    ) : IRequest<AuthResponse>, IAuditable
    {
        public string EntityName => "User";
        public Guid EntityId => Guid.Empty;
        public string Action => "Register";
        public string Details => $"User registered from IP {IpAddress}";
        public string PerformedBy => Email;
    }
} 