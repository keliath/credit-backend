using System;

namespace CreditApp.Application.DTOs
{
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
} 