using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CreditApp.Application.DTOs;
using CreditApp.Domain.Entities;
using CreditApp.Domain.ValueObjects;
using CreditApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CreditApp.Application.Services
{
    public class CreditRequestService : ICreditRequestService
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuditService _auditService;

        public CreditRequestService(ApplicationDbContext context, IAuditService auditService)
        {
            _context = context;
            _auditService = auditService;
        }

        public async Task<CreditRequest> GetByIdAsync(Guid id)
        {
            var entity = await _context.CreditRequests
                .Include(cr => cr.User)
                .FirstOrDefaultAsync(cr => cr.Id == id);
            if (entity == null)
                throw new KeyNotFoundException("No se encontró la solicitud de crédito");
            return entity;
        }

        public async Task<IEnumerable<CreditRequest>> GetByUserIdAsync(Guid userId)
        {
            return await _context.CreditRequests
                .Include(cr => cr.User)
                .Where(cr => cr.UserId == userId)
                .OrderByDescending(cr => cr.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<CreditRequest>> GetAllAsync(string? status = null)
        {
            var query = _context.CreditRequests
                .Include(cr => cr.User)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(cr => cr.Status == status);
            }

            return await query
                .OrderByDescending(cr => cr.CreatedAt)
                .ToListAsync();
        }

        public async Task<PagedResult<CreditRequest>> GetAllPagedAsync(int page = 1, int size = 10, string? status = null)
        {
            var query = _context.CreditRequests
                .Include(cr => cr.User)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(cr => cr.Status == status);
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(cr => cr.CreatedAt)
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync();

            return new PagedResult<CreditRequest>
            {
                Items = items,
                Page = page,
                Size = size,
                TotalCount = totalCount
            };
        }
    }
} 