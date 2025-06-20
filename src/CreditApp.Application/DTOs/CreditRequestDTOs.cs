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

    public record CreditRequestDetailResponse(
        Guid Id,
        Guid UserId,
        UserInfo User,
        MoneyInfo Amount,
        MoneyInfo MonthlyIncome,
        int WorkSeniorityYears,
        int TermInMonths,
        string Purpose,
        string Status,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        string? RejectionReason,
        string? ApprovedBy,
        DateTime? ApprovedAt
    );

    public record CreditRequestMyRequestsResponse(
        Guid Id,
        UserInfo User,
        MoneyInfo Amount,
        MoneyInfo MonthlyIncome,
        int WorkSeniorityYears,
        int TermInMonths,
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

    public record CreditRequestAnalystResponse(
        Guid Id,
        UserInfo User,
        MoneyInfo Amount,
        MoneyInfo MonthlyIncome,
        int WorkSeniorityYears,
        int TermInMonths,
        string Purpose,
        string Status,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        string? RejectionReason,
        string? ApprovedBy,
        DateTime? ApprovedAt
    );

    public record UserInfo(
        string Username,
        string Email
    );

    public record MoneyInfo(
        decimal Amount,
        string Currency
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

    public record CreditRequestExportDTO
    {
        public Guid Id { get; init; }
        public string Username { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public decimal Amount { get; init; }
        public decimal MonthlyIncome { get; init; }
        public int TermInMonths { get; init; }
        public int WorkSeniorityYears { get; init; }
        public string Purpose { get; init; } = string.Empty;
        public string Status { get; init; } = string.Empty;
        public string? RejectionReason { get; init; }
        public string? ApprovedBy { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime? ApprovedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
    }

    public class ExportCreditRequestsRequest
    {
        public string? Status { get; set; }
        public string? Format { get; set; } = "excel"; // excel, csv
    }
} 