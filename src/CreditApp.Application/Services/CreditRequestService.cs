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

        public CreditRequestService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CreditRequest> CreateAsync(
            Guid userId,
            decimal amount,
            string currency,
            int termInMonths,
            decimal monthlyIncome,
            string monthlyIncomeCurrency,
            int workSeniorityYears,
            string purpose)
        {
            var request = new CreditRequest(
                userId,
                Money.Create(amount, currency),
                termInMonths,
                Money.Create(monthlyIncome, monthlyIncomeCurrency),
                workSeniorityYears,
                purpose);

            _context.CreditRequests.Add(request);
            await _context.SaveChangesAsync();
            return request;
        }

        public async Task<CreditRequest> GetByIdAsync(Guid id)
        {
            return await _context.CreditRequests
                .Include(cr => cr.User)
                .FirstOrDefaultAsync(cr => cr.Id == id);
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

        public async Task<CreditRequest> UpdateStatusAsync(
            Guid id,
            string newStatus,
            string? rejectionReason = null,
            string? approvedBy = null)
        {
            var request = await _context.CreditRequests.FindAsync(id);
            if (request == null)
                throw new KeyNotFoundException("Credit request not found");

            request.UpdateStatus(newStatus, rejectionReason, approvedBy);
            await _context.SaveChangesAsync();
            return request;
        }
    }
} 