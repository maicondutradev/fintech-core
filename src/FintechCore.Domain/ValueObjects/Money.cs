using FintechCore.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace FintechCore.Domain.ValueObjects
{
    public sealed record Money
    {
        public decimal Amount { get;}
        public string Currency {  get;}

        private Money(decimal amount, string currency)
        {
            Amount = amount;
            Currency = currency;
        }

        public static Result<Money> Create (decimal amount, string currency)
        {
            if(string.IsNullOrWhiteSpace(currency) || currency.Trim().Length != 3)
            {
                return Result.Failure<Money>(Error.Validation("Money.InvalidCurrency", "Currency must be a 3-Letter ISO code."));
            }

            if(amount < 0)
            {
                return Result.Failure<Money>(Error.Validation("Money.NegativeAmount", "Amount cannot be negative."));
            }

            return Result.Success(new Money(decimal.Round(amount,2), currency.Trim().ToUpperInvariant()));
        }
    }
}
