using System;
using System.Collections.Generic;
using MediatR;

namespace CreditApp.Application.DTOs
{
    public interface IAuditable
    {
        string EntityName { get; }
        Guid EntityId { get; }
        string Action { get; }
        string Details { get; }
        string PerformedBy { get; }
    }

    public record PaginationParams(int Page = 1, int Size = 10);

    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();
        public int Page { get; set; }
        public int Size { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / Size);
    }

    public record CreateCreditRequestRequest(
        decimal Amount,
        string Purpose
    );

    public record CreditRequestResponse(
        Guid Id,
        Guid UserId,
        decimal Amount,
        string Purpose,
        string Status,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        string? RejectionReason,
        string? ApprovedBy,
        DateTime? ApprovedAt
    );

    public record UpdateCreditRequestStatusRequest(
        string Status,
        string? RejectionReason = null
    );

    public record CreditRequestListResponse(
        Guid Id,
        string Username,
        decimal Amount,
        string Purpose,
        string Status,
        DateTime CreatedAt,
        DateTime? UpdatedAt
    );

    // MediatR Command
    public record UpdateCreditRequestStatusCommand(
        Guid Id,
        string NewStatus,
        string? RejectionReason,
        string? ApprovedBy
    ) : IRequest<CreditRequestResponse>, IAuditable
    {
        public string EntityName => "CreditRequest";
        public Guid EntityId => Id;
        public string Action => "StatusUpdate";
        public string Details => $"Status changed to {NewStatus}";
        public string PerformedBy => ApprovedBy ?? "System";
    }

    public record CreateCreditRequestCommand(
        Guid UserId,
        decimal Amount,
        string Currency,
        int TermInMonths,
        decimal MonthlyIncome,
        string MonthlyIncomeCurrency,
        int WorkSeniorityYears,
        string Purpose,
        string Username
    ) : IRequest<CreditRequestResponse>, IAuditable
    {
        public string EntityName => "CreditRequest";
        public Guid EntityId { get; init; } = Guid.Empty; // Se asignará en el handler
        public string Action => "Create";
        public string Details => $"Credit request created for amount {Amount} {Currency}";
        public string PerformedBy => Username;
    }

    public record DeleteCreditRequestCommand(
        Guid Id,
        string Username
    ) : IRequest<bool>, IAuditable
    {
        public string EntityName => "CreditRequest";
        public Guid EntityId => Id;
        public string Action => "Delete";
        public string Details => $"Credit request deleted";
        public string PerformedBy => Username;
    }
} 