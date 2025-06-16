
using CreditApp.Domain.Abstractions;

namespace CreditApp.Domain.CreditRequests;

public sealed class CreditRequest : Entity
{
    public Money Amount { get; private set; }
    public int TermMonths { get; private set; }
    public Money MonthlyIncome { get; private set; }
    public int WorkSeniority { get; private set; }
    public CreditState State { get; private set; }

    private CreditRequest() : base(Guid.NewGuid())
    {
        Amount = Money.Zero();
        MonthlyIncome = Money.Zero();
        // para EF
    }

    public CreditRequest(Guid id, Money amount, int termMonths, Money monthlyIncome, int workSeniority)
        : base(id)
    {
        Amount = amount;
        TermMonths = termMonths;
        MonthlyIncome = monthlyIncome;
        WorkSeniority = workSeniority;
        State = SuggestInitialState(monthlyIncome);
    }

    private CreditState SuggestInitialState(Money income)
        => income.Amount >= 1500m ? CreditState.Approved : CreditState.Pending;

    public void ChangeState(CreditState newState)
    {
        // aquí podrías validar transiciones, lanzar dominio-events, etc.
        State = newState;
    }
}

public enum CreditState { Pending, Approved, Rejected }
