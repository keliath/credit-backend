namespace CreditApp.Domain.CreditRequests;

public record Money(decimal Amount)
{
    public static Money operator +(Money primero, Money segundo)
    {
        return new Money(primero.Amount + segundo.Amount);
    }

    public static Money Zero() => new(0);
    public bool IsZero() => this == Zero();
}
