using System;
using System.Collections.Generic;
using CreditApp.Domain.Enums;
using CreditApp.Domain.Abstractions;

namespace CreditApp.Domain.Entities
{
    public class User : Entity
    {
        public new Guid Id { get; private set; }
        public string Username { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string PasswordHash { get; private set; } = string.Empty;
        public UserRole Role { get; private set; }
        public new DateTime CreatedAt { get; private set; }
        public DateTime? LastLoginAt { get; private set; }
        public bool IsActive { get; private set; }

        public ICollection<CreditRequest> CreditRequests { get; private set; } = new List<CreditRequest>();

        private User() { } // For EF Core

        public User(string username, string email, string passwordHash, UserRole role)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("El nombre de usuario no puede estar vacío", nameof(username));

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("El correo electrónico no puede estar vacío", nameof(email));

            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("La contraseña no puede estar vacía", nameof(passwordHash));

            Id = Guid.NewGuid();
            Username = username;
            Email = email;
            PasswordHash = passwordHash;
            Role = role;
            CreatedAt = DateTime.UtcNow;
            IsActive = true;
            CreditRequests = new List<CreditRequest>();
        }

        public void UpdateLastLogin()
        {
            LastLoginAt = DateTime.UtcNow;
            UpdateTimestamp();
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdateTimestamp();
        }

        public void Activate()
        {
            IsActive = true;
            UpdateTimestamp();
        }

        public void UpdateEmail(string newEmail)
        {
            if (string.IsNullOrWhiteSpace(newEmail))
                throw new ArgumentException("El correo electrónico no puede estar vacío", nameof(newEmail));

            Email = newEmail;
        }

        public void UpdatePassword(string newPasswordHash)
        {
            if (string.IsNullOrWhiteSpace(newPasswordHash))
                throw new ArgumentException("La contraseña no puede estar vacía", nameof(newPasswordHash));

            PasswordHash = newPasswordHash;
        }
    }
} 