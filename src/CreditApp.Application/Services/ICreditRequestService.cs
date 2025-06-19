using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CreditApp.Domain.Entities;
using CreditApp.Application.DTOs;

namespace CreditApp.Application.Services
{
    public interface ICreditRequestService
    {
        Task<CreditRequest> GetByIdAsync(Guid id);
        Task<IEnumerable<CreditRequest>> GetByUserIdAsync(Guid userId);
        Task<IEnumerable<CreditRequest>> GetAllAsync(string? status = null);
        Task<PagedResult<CreditRequest>> GetAllPagedAsync(int page = 1, int size = 10, string? status = null);
    }
} 