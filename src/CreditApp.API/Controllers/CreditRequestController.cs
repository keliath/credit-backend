using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CreditApp.Application.Services;
using System.Security.Claims;
using CreditApp.Domain.Entities;

namespace CreditApp.API.Controllers
{
    // [Authorize]
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
        public async Task<ActionResult<CreditRequest>> Create(CreateCreditRequestRequest request)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var response = await _creditRequestService.CreateAsync(
                    userId,
                    request.Amount,
                    request.Currency,
                    request.TermInMonths,
                    request.MonthlyIncome,
                    request.MonthlyIncomeCurrency,
                    request.WorkSeniorityYears,
                    request.Purpose);
                return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
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
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Analyst")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CreditRequest>>> GetAll()
        {
            var response = await _creditRequestService.GetAllAsync();
            return Ok(response);
        }

        [HttpGet("my-requests")]
        public async Task<ActionResult<IEnumerable<CreditRequest>>> GetMyRequests()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var response = await _creditRequestService.GetByUserIdAsync(userId);
            return Ok(response);
        }

        [Authorize(Roles = "Analyst")]
        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<CreditRequest>>> GetByStatus(string status)
        {
            var response = await _creditRequestService.GetByStatusAsync(status);
            return Ok(response);
        }

        [Authorize(Roles = "Analyst")]
        [HttpPut("{id}/status")]
        public async Task<ActionResult<CreditRequest>> UpdateStatus(
            Guid id,
            [FromBody] UpdateCreditRequestStatusRequest request)
        {
            try
            {
                var approvedBy = User.FindFirst(ClaimTypes.Name)?.Value;
                var response = await _creditRequestService.UpdateStatusAsync(
                    id,
                    request.Status,
                    request.RejectionReason,
                    approvedBy);

                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
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