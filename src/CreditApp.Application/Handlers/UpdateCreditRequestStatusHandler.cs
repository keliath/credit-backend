using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using CreditApp.Application.DTOs;
using CreditApp.Infrastructure.Data;
using CreditApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CreditApp.Application.Handlers
{
    public class UpdateCreditRequestStatusHandler : IRequestHandler<UpdateCreditRequestStatusCommand, CreditRequestResponse>
    {
        private readonly ApplicationDbContext _context;

        public UpdateCreditRequestStatusHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CreditRequestResponse> Handle(UpdateCreditRequestStatusCommand request, CancellationToken cancellationToken)
        {
            var creditRequest = await _context.CreditRequests.FindAsync(request.Id);
            if (creditRequest == null)
                throw new KeyNotFoundException("Credit request not found");

            creditRequest.UpdateStatus(request.NewStatus, request.RejectionReason ?? string.Empty, request.ApprovedBy ?? string.Empty);
            await _context.SaveChangesAsync(cancellationToken);

            return new CreditRequestResponse(
                creditRequest.Id,
                creditRequest.UserId,
                creditRequest.Amount.Amount,
                creditRequest.Purpose,
                creditRequest.Status,
                creditRequest.CreatedAt,
                creditRequest.UpdatedAt,
                creditRequest.RejectionReason,
                creditRequest.ApprovedBy,
                creditRequest.ApprovedAt
            );
        }
    }
} 