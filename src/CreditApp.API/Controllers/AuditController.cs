using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CreditApp.Application.Services;
using CreditApp.Domain.Entities;

namespace CreditApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Analista")]
public class AuditController : ControllerBase
{
    private readonly IAuditService _auditService;

    public AuditController(IAuditService auditService)
    {
        _auditService = auditService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AuditLog>>> GetAuditLogs(
        [FromQuery] string? entityName = null,
        [FromQuery] Guid? entityId = null,
        [FromQuery] string? action = null,
        [FromQuery] string? performedBy = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var logs = await _auditService.GetAuditLogsAsync(entityName, entityId, action, performedBy, startDate, endDate);
        return Ok(logs);
    }
} 