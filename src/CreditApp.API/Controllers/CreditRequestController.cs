using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CreditApp.Application.Services;
using CreditApp.Application.DTOs;
using System.Security.Claims;
using CreditApp.Domain.Entities;
using MediatR;
using CreditApp.Application.Handlers;

namespace CreditApp.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CreditRequestController : ControllerBase
    {
        private readonly ICreditRequestService _creditRequestService;

        public CreditRequestController(ICreditRequestService creditRequestService)
        {
            _creditRequestService = creditRequestService;
        }

        [HttpPost]
        public async Task<ActionResult<CreditRequestResponse>> Create(CreateCreditRequestRequest request, [FromServices] IMediator mediator)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var username = User.FindFirst(ClaimTypes.Name)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || string.IsNullOrEmpty(username))
                    return Unauthorized(new { message = "No se pudo identificar al usuario" });
                var userId = Guid.Parse(userIdClaim);
                var command = new CreateCreditRequestCommand(
                    userId,
                    request.Amount,
                    request.Currency,
                    request.TermInMonths,
                    request.MonthlyIncome,
                    request.MonthlyIncomeCurrency,
                    request.WorkSeniorityYears,
                    request.Purpose,
                    username
                );
                var response = await mediator.Send(command);
                return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "No se encontró la solicitud de crédito" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CreditRequestDetailResponse>> GetById(Guid id)
        {
            try
            {
                var cr = await _creditRequestService.GetByIdAsync(id);
                var dto = new CreditRequestDetailResponse(
                    cr.Id,
                    cr.UserId,
                    new UserInfo(cr.User.Username, cr.User.Email),
                    new MoneyInfo(cr.Amount.Amount, cr.Amount.Currency),
                    new MoneyInfo(cr.MonthlyIncome.Amount, cr.MonthlyIncome.Currency),
                    cr.WorkSeniorityYears,
                    cr.TermInMonths,
                    cr.Purpose,
                    cr.Status,
                    cr.CreatedAt,
                    cr.UpdatedAt,
                    string.IsNullOrEmpty(cr.RejectionReason) ? null : cr.RejectionReason,
                    string.IsNullOrEmpty(cr.ApprovedBy) ? null : cr.ApprovedBy,
                    cr.ApprovedAt
                );
                return Ok(dto);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "No se encontró la solicitud de crédito" });
            }
        }

        [Authorize(Roles = "Analyst")]
        [HttpGet]
        public async Task<ActionResult<PagedResult<CreditRequestAnalystResponse>>> GetAll([FromQuery] string? status = null, [FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            var response = await _creditRequestService.GetAllPagedAsync(page, size, status);
            var dto = new PagedResult<CreditRequestAnalystResponse>
            {
                Items = response.Items.Select(cr => new CreditRequestAnalystResponse(
                    cr.Id,
                    new UserInfo(cr.User.Username, cr.User.Email),
                    new MoneyInfo(cr.Amount.Amount, cr.Amount.Currency),
                    new MoneyInfo(cr.MonthlyIncome.Amount, cr.MonthlyIncome.Currency),
                    cr.WorkSeniorityYears,
                    cr.TermInMonths,
                    cr.Purpose,
                    cr.Status,
                    cr.CreatedAt,
                    cr.UpdatedAt,
                    string.IsNullOrEmpty(cr.RejectionReason) ? null : cr.RejectionReason,
                    string.IsNullOrEmpty(cr.ApprovedBy) ? null : cr.ApprovedBy,
                    cr.ApprovedAt
                )),
                Page = response.Page,
                Size = response.Size,
                TotalCount = response.TotalCount
            };
            return Ok(dto);
        }

        [HttpGet("my-requests")]
        public async Task<ActionResult<IEnumerable<CreditRequestMyRequestsResponse>>> GetMyRequests()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized(new { message = "No se pudo identificar al usuario" });
            var userId = Guid.Parse(userIdClaim);
            var response = await _creditRequestService.GetByUserIdAsync(userId);
            var dto = response.Select(cr => new CreditRequestMyRequestsResponse(
                cr.Id,
                new UserInfo(cr.User.Username, cr.User.Email),
                new MoneyInfo(cr.Amount.Amount, cr.Amount.Currency),
                new MoneyInfo(cr.MonthlyIncome.Amount, cr.MonthlyIncome.Currency),
                cr.WorkSeniorityYears,
                cr.TermInMonths,
                cr.Purpose,
                cr.Status,
                cr.CreatedAt,
                cr.UpdatedAt,
                string.IsNullOrEmpty(cr.RejectionReason) ? null : cr.RejectionReason,
                string.IsNullOrEmpty(cr.ApprovedBy) ? null : cr.ApprovedBy,
                cr.ApprovedAt
            ));
            return Ok(dto);
        }

        [Authorize(Roles = "Analyst")]
        [HttpPut("{id}/status")]
        public async Task<ActionResult<CreditRequestResponse>> UpdateStatus(
            Guid id,
            [FromBody] UpdateCreditRequestStatusRequest request,
            [FromServices] IMediator mediator)
        {
            try
            {
                var approvedBy = User.FindFirst(ClaimTypes.Name)?.Value;
                if (string.IsNullOrEmpty(approvedBy))
                    return Unauthorized(new { message = "No se pudo identificar al usuario" });
                var command = new UpdateCreditRequestStatusCommand(
                    id,
                    request.Status,
                    request.RejectionReason,
                    approvedBy
                );
                var response = await mediator.Send(command);
                return Ok(response);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "No se encontró la solicitud de crédito" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id, [FromServices] IMediator mediator)
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(username))
                return Unauthorized(new { message = "No se pudo identificar al usuario" });
            var command = new DeleteCreditRequestCommand(id, username);
            var result = await mediator.Send(command);
            if (!result)
                return NotFound(new { message = "No se encontró la solicitud de crédito" });
            return NoContent();
        }

        [Authorize(Roles = "Analyst")]
        [HttpPost("export")]
        public async Task<IActionResult> ExportToExcel(
            [FromServices] IMediator mediator,
            [FromBody] ExportCreditRequestsRequest? request = null)
        {
            try
            {
                // Si no se envía request, crear uno por defecto
                var exportRequest = request ?? new ExportCreditRequestsRequest();
                
                var query = new ExportCreditRequestsQuery(exportRequest);
                var excelBytes = await mediator.Send(query);
                
                var fileName = $"solicitudes_credito_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                
                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error al generar el reporte: {ex.Message}" });
            }
        }

        [Authorize(Roles = "Analyst")]
        [HttpGet("export")]
        public async Task<IActionResult> ExportToExcelGet(
            [FromServices] IMediator mediator,
            [FromQuery] string? status = null,
            [FromQuery] string? format = "excel")
        {
            try
            {
                var exportRequest = new ExportCreditRequestsRequest
                {
                    Status = status,
                    Format = format
                };
                
                var query = new ExportCreditRequestsQuery(exportRequest);
                var excelBytes = await mediator.Send(query);
                
                var fileName = $"solicitudes_credito_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                
                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error al generar el reporte: {ex.Message}" });
            }
        }
    }

    public record CreateCreditRequestRequest(
        decimal Amount,
        string Currency,
        int TermInMonths,
        decimal MonthlyIncome,
        string MonthlyIncomeCurrency,
        int WorkSeniorityYears,
        string Purpose
    );

    public record UpdateCreditRequestStatusRequest(
        string Status,
        string RejectionReason = null
    );
} 