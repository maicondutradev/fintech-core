using FintechCore.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace FintechCore.Domain.ValueObjects
{
    public sealed record AccountNumber
    {
        public string Value { get; }

        private AccountNumber(string value)
        {
            Value = value;
        }

        public static Result<AccountNumber> Create (string value)
        {
            if(string.IsNullOrWhiteSpace(value))
            {
                return Result.Failure <AccountNumber>(Error.Validation("AccountNumber.Empty", "Account number cannot be empty."));
            }

            string trimmedValue = value.Trim();

            if(trimmedValue.Length < 5 || trimmedValue.Length  > 20)
            {

                return Result.Failure<AccountNumber>(Error.Validation("AccountNumber.InvalidLength", "Account number must be between 5 and 20 characters."));
            }

            return Result.Success(new AccountNumber(trimmedValue));
        }
    }
}
