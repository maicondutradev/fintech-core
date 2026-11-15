using FintechCore.Domain.Common;
using FintechCore.Domain.Enums;
using FintechCore.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace FintechCore.Domain.Entities
{
    public sealed class PostingLine
    {
        public Guid Id { get; }
        public Guid LedgerEntryId { get; }
        public Guid AccountId { get; }
        public Money Amount { get; }
        public  PostingDirection Direction { get;}
        public DateTimeOffset CreatedAt { get; }

        private PostingLine() { Amount = null!; }
        private PostingLine(Guid ledgerEntryId, Guid accountId, Money amount, PostingDirection direction)
        {
            Id = Guid.CreateVersion7();
            LedgerEntryId = ledgerEntryId;
            AccountId = accountId;
            Amount = amount;
            Direction = direction;
            CreatedAt = DateTimeOffset.UtcNow;
        }

        public static Result<PostingLine> Create(Guid ledgerEntryId, Guid accountId, Money amount, PostingDirection direction)
        {
            if(ledgerEntryId == Guid.Empty)
            {
                return Result.Failure<PostingLine>(Error.Validation("PostingLine.InvalidLedgerEntry", "LedgerEntryId cannot be empty."));
            }
            if(accountId == Guid.Empty)
            {
                return Result.Failure<PostingLine>(Error.Validation("PostingLine.InvalidAccount", "AccountId cannot be empty."));
            }
            if (amount is null || amount.Amount <= 0)
            {
                return Result.Failure<PostingLine>(Error.Validation("PostingLine.ZeroAmount", "Posting amount must be greater than zero."));
            }

            return Result.Success<PostingLine>(new PostingLine(ledgerEntryId, accountId, amount, direction));
        }
    }
}
