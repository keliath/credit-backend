using System.Threading;
using System.Threading.Tasks;
using MediatR;
using CreditApp.Application.DTOs;
using CreditApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CreditApp.Application.Handlers
{
    public class DeleteCreditRequestHandler : IRequestHandler<DeleteCreditRequestCommand, bool>
    {
        private readonly ApplicationDbContext _context;

        public DeleteCreditRequestHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteCreditRequestCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.CreditRequests.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (entity == null)
                return false;

            _context.CreditRequests.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
} 