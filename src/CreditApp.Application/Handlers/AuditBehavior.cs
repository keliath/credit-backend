using System.Threading;
using System.Threading.Tasks;
using MediatR;
using CreditApp.Application.DTOs;
using CreditApp.Application.Services;

namespace CreditApp.Application.Handlers
{
    public class AuditBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IAuditService _auditService;

        public AuditBehavior(IAuditService auditService)
        {
            _auditService = auditService;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var response = await next();

            if (request is IAuditable auditable)
            {
                await _auditService.LogActionAsync(
                    auditable.EntityName,
                    auditable.EntityId,
                    auditable.Action,
                    auditable.Details,
                    auditable.PerformedBy
                );
            }

            return response;
        }
    }
} 