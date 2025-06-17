using CreditApp.Domain.Entities;

namespace CreditApp.Application.Services;

public interface IAuditService
{
    Task LogActionAsync(string entityName, Guid entityId, string action, string details, string performedBy);
    Task<IEnumerable<AuditLog>> GetAuditLogsAsync(string? entityName = null, Guid? entityId = null, string? action = null, string? performedBy = null, DateTime? startDate = null, DateTime? endDate = null);
} 