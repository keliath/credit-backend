using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CreditApp.Domain.Entities;

namespace CreditApp.Application.Services
{
    public interface ICreditRequestService
    {
        Task<CreditRequest> CreateAsync(
            Guid userId,
            decimal amount,
            string currency,
            int termInMonths,
            decimal monthlyIncome,
            string monthlyIncomeCurrency,
            int workSeniorityYears,
            string purpose);

        Task<CreditRequest> GetByIdAsync(Guid id);
        Task<IEnumerable<CreditRequest>> GetByUserIdAsync(Guid userId);
        Task<IEnumerable<CreditRequest>> GetAllAsync();
        Task<IEnumerable<CreditRequest>> GetByStatusAsync(string status);
        Task<CreditRequest> UpdateStatusAsync(
            Guid id,
            string newStatus,
            string? rejectionReason = null,
            string? approvedBy = null);
    }
} 