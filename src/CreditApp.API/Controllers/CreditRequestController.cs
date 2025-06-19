using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CreditApp.Application.Services;
using CreditApp.Application.DTOs;
using System.Security.Claims;
using CreditApp.Domain.Entities;
using MediatR;

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
        public async Task<ActionResult<CreditRequest>> GetById(Guid id)
        {
            try
            {
                var response = await _creditRequestService.GetByIdAsync(id);
                return Ok(response);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "No se encontró la solicitud de crédito" });
            }
        }

        [Authorize(Roles = "Analyst")]
        [HttpGet]
        public async Task<ActionResult<PagedResult<CreditRequest>>> GetAll([FromQuery] string? status = null, [FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            var response = await _creditRequestService.GetAllPagedAsync(page, size, status);
            return Ok(response);
        }

        [HttpGet("my-requests")]
        public async Task<ActionResult<IEnumerable<CreditRequest>>> GetMyRequests()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                return Unauthorized(new { message = "No se pudo identificar al usuario" });
            var userId = Guid.Parse(userIdClaim);
            var response = await _creditRequestService.GetByUserIdAsync(userId);
            return Ok(response ?? new List<CreditRequest>());
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