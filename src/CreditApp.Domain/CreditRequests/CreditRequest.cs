using CreditApp.Domain.Abstractions;

namespace CreditApp.Domain.CreditRequests;

public class CreditRequest : Entity
{
    public decimal Amount { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public CreditRequestStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private CreditRequest() { } // For EF

    public CreditRequest(decimal amount, string description)
    {
        Amount = amount;
        Description = description;
        Status = CreditRequestStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateStatus(CreditRequestStatus newStatus)
    {
        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;
    }
} 