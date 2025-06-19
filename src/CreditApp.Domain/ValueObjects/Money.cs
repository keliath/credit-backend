using System;

namespace CreditApp.Domain.ValueObjects
{
    public class Money : IEquatable<Money>
    {
        public decimal Amount { get; }
        public string Currency { get; }

        private Money(decimal amount, string currency)
        {
            if (amount < 0)
                throw new ArgumentException("El monto no puede ser negativo", nameof(amount));
            
            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("La moneda no puede estar vacía", nameof(currency));

            Amount = amount;
            Currency = currency.ToUpper();
        }

        public static Money Create(decimal amount, string currency = "USD")
        {
            return new Money(amount, currency);
        }

        public static Money operator +(Money left, Money right)
        {
            if (left.Currency != right.Currency)
                throw new InvalidOperationException("No se puede sumar dinero con diferentes monedas");

            return new Money(left.Amount + right.Amount, left.Currency);
        }

        public static Money operator -(Money left, Money right)
        {
            if (left.Currency != right.Currency)
                throw new InvalidOperationException("No se puede restar dinero con diferentes monedas");

            return new Money(left.Amount - right.Amount, left.Currency);
        }

        public static Money operator *(Money money, decimal multiplier)
        {
            return new Money(money.Amount * multiplier, money.Currency);
        }

        public static Money operator /(Money money, decimal divisor)
        {
            if (divisor == 0)
                throw new DivideByZeroException();

            return new Money(money.Amount / divisor, money.Currency);
        }

        public bool Equals(Money? other)
        {
            if (other is null) return false;
            return Amount == other.Amount && Currency == other.Currency;
        }

        public override bool Equals(object? obj)
        {
            if (obj is Money other)
                return Equals(other);
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Amount, Currency);
        }

        public override string ToString()
        {
            return $"{Amount:C} {Currency}";
        }

        public static bool operator ==(Money? left, Money? right)
        {
            if (left is null && right is null) return true;
            if (left is null || right is null) return false;
            return left.Equals(right);
        }

        public static bool operator !=(Money? left, Money? right)
        {
            return !(left == right);
        }
    }
} 