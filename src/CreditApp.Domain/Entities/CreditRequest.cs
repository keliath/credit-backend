using System;
using CreditApp.Domain.Abstractions;
using CreditApp.Domain.ValueObjects;

namespace CreditApp.Domain.Entities
{
    public class CreditRequest : Entity
    {
        public Guid UserId { get; private set; }
        public User User { get; private set; }
        public Money Amount { get; private set; }
        public int TermInMonths { get; private set; }
        public Money MonthlyIncome { get; private set; }
        public int WorkSeniorityYears { get; private set; }
        public string Purpose { get; private set; }
        public string Status { get; private set; }
        public string RejectionReason { get; private set; }
        public string ApprovedBy { get; private set; }
        public DateTime? ApprovedAt { get; private set; }

        private CreditRequest() { } // For EF Core

        public CreditRequest(
            Guid userId,
            Money amount,
            int termInMonths,
            Money monthlyIncome,
            int workSeniorityYears,
            string purpose
        )
        {
            UserId = userId;
            Amount = amount;
            TermInMonths = termInMonths;
            MonthlyIncome = monthlyIncome;
            WorkSeniorityYears = workSeniorityYears;
            Purpose = purpose;
            Status = "Pending";
            RejectionReason = string.Empty;
            ApprovedBy = string.Empty;
        }

        public void UpdateStatus(string newStatus, string rejectionReason, string approvedBy)
        {
            if (string.IsNullOrWhiteSpace(newStatus))
                throw new ArgumentException("Status cannot be empty", nameof(newStatus));

            Status = newStatus;
            RejectionReason = rejectionReason ?? string.Empty;
            ApprovedBy = approvedBy ?? string.Empty;
            ApprovedAt = DateTime.UtcNow;
            UpdateTimestamp();
        }
    }
} 