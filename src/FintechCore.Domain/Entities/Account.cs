using FintechCore.Domain.Common;
using FintechCore.Domain.Enums;
using FintechCore.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace FintechCore.Domain.Entities
{
    public sealed class Account : ITenantEntity
    {
        public Guid Id { get; private set; }
        public Guid TenantId { get; private set; }
        public AccountNumber AccountNumber { get; private set; }
        public string Currency {  get; private set; }
        public AccountType Type { get; private set; }
        public decimal CurrentBalance { get; private set; }
        public uint Version { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }
        public DateTimeOffset UpdatedAt { get; private set; }
        private Account() { AccountNumber = null!; Currency = null!; }
        private Account(Guid tenantId, AccountNumber accountNumber, string currency, AccountType type)
        {
            Id = Guid.CreateVersion7();
            TenantId = tenantId;
            AccountNumber = accountNumber;
            Currency = currency;
            Type = type;
            CurrentBalance = 0m;
            Version = 1;
            CreatedAt = DateTimeOffset.UtcNow;
            UpdatedAt = CreatedAt;
        }
        public static Result<Account> Create(Guid tenantId, AccountNumber accountNumber, string currency, AccountType type)
        {
            if(tenantId == Guid.Empty)
            {
                return Result.Failure<Account>(Error.Validation("Account.InvalidTenant", "Tenant ID cannot be empty."));
            }
            if (accountNumber is null)
            {
                return Result.Failure<Account>(Error.Validation("Account.InvalidAccountNumber", "Account number is required."));
            }
            if (string.IsNullOrWhiteSpace(currency) || currency.Trim().Length != 3)
            {
                return Result.Failure<Account>(Error.Validation("Account.InvalidCurrency", "Currency must be a 3-letter ISO code."));
            }

            return Result.Success<Account>(new Account(tenantId, accountNumber, currency.Trim().ToUpperInvariant(), type));
        }
        public Result Credit(Money amount)
        {
            if(amount is null || amount.Amount <= 0)
            {
                return Result.Failure(Error.Validation("Account.InvalidAmount", "Operation amount must be greater than zero."));
            }
            if (amount.Currency != Currency)
            {
                return Result.Failure(Error.Validation("Account.CurrencyMismatch", "Operation currency does not match the account currency."));
            }

            CurrentBalance += amount.Amount;
            UpdatedAt = DateTimeOffset.UtcNow;
            Version++;

            return Result.Success();
        }

        public Result Debit (Money amount)
        {
            if (amount is null || amount.Amount <= 0)
            {
                return Result.Failure(Error.Validation("Account.InvalidAmount", "Operation amount must be greater than zero."));
            }
            if (amount.Currency != Currency)
            {
                return Result.Failure(Error.Validation("Account.CurrencyMismatch", "Operation currency does not match the account currency."));
            }
            if (Type == AccountType.Asset && CurrentBalance < amount.Amount)
            {
                return Result.Failure(Error.Failure("Account.InsufficientFunds", "Insufficient funds for this operation."));
            }

            CurrentBalance -= amount.Amount;
            UpdatedAt = DateTimeOffset.UtcNow;
            Version++;
            return Result.Success();
        }
    }
}
