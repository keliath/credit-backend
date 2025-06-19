using System.Threading;
using System.Threading.Tasks;
using MediatR;
using CreditApp.Application.DTOs;
using CreditApp.Infrastructure.Data;
using CreditApp.Domain.Entities;
using CreditApp.Domain.ValueObjects;

namespace CreditApp.Application.Handlers
{
    public class CreateCreditRequestHandler : IRequestHandler<CreateCreditRequestCommand, CreditRequestResponse>
    {
        private readonly ApplicationDbContext _context;

        public CreateCreditRequestHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CreditRequestResponse> Handle(CreateCreditRequestCommand request, CancellationToken cancellationToken)
        {
            var creditRequest = new CreditRequest(
                request.UserId,
                Money.Create(request.Amount, request.Currency),
                request.TermInMonths,
                Money.Create(request.MonthlyIncome, request.MonthlyIncomeCurrency),
                request.WorkSeniorityYears,
                request.Purpose
            );

            _context.CreditRequests.Add(creditRequest);
            await _context.SaveChangesAsync(cancellationToken);

            // Asignar el EntityId para la auditoría
            request = request with { EntityId = creditRequest.Id };

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