using FintechCore.Domain.Common;
using FintechCore.Domain.Enums;
using FintechCore.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace FintechCore.Domain.Entities
{
    public sealed class LedgerEntry : ITenantEntity
    {
        private readonly List<PostingLine> _postingLines = [];

        public IReadOnlyCollection<PostingLine> PostingLines => _postingLines.AsReadOnly();

        public Guid Id { get; private set; }
        public Guid TenantId { get; private set; }
        public string TransactionReference { get; private set; }
        public string OperationType { get; private set; }
        public string Description { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }

        private LedgerEntry() { TransactionReference = null!;
        OperationType = null!; Description = null!; }

        private LedgerEntry (Guid id, Guid tenantId, 
            string transactionReference, string operationType, 
            string description, List<PostingLine> postingLines)
        {
            Id = id;
            TenantId = tenantId;
            TransactionReference = transactionReference;
            OperationType = operationType;
            Description = description;
            CreatedAt = DateTimeOffset.UtcNow;
            _postingLines.AddRange(postingLines);
        }

        public static Result<LedgerEntry> Create(Guid id, Guid tenantId, string transactionReference,
            string operationType, string description, IReadOnlyCollection<PostingLine> postingLines)
        {
            if(id == Guid.Empty)
            {
                return Result.Failure<LedgerEntry>(Error.Validation("LedgerEntry.InvalidId", "Ledger entry ID cannot be empty."));
            }
            if(tenantId == Guid.Empty)
            {
                return Result.Failure<LedgerEntry>(Error.Validation("LedgerEntry.InvalidTenant", "Tenant ID cannot be empty."));
            }
            if(string.IsNullOrWhiteSpace(transactionReference))
            {
                return Result.Failure<LedgerEntry>(Error.Validation("LedgerEntry.InvalidReference", "Transaction reference is required."));
            }
            if (string.IsNullOrWhiteSpace(operationType))
            {
                return Result.Failure<LedgerEntry>(Error.Validation("LedgerEntry.InvalidOperationType", "Operation type is required."));
            }
            if(postingLines is null || postingLines.Count < 2)
            {
                return Result.Failure<LedgerEntry>(Error.Validation("LedgerEntry.InsufficientLines", "A ledger entry must contain at least two posting lines."));
            }
            
            string currency = postingLines.First().Amount.Currency;

            if (postingLines.Any(x => x.Amount.Currency != currency))
            {
                return Result.Failure<LedgerEntry>(Error.Validation("LedgerEntry.CurrencyMismatch", "All posting lines must have the same currency."));
            }
            if(postingLines.Any(x => x.LedgerEntryId != id))
            {
                return Result.Failure<LedgerEntry>(Error.Validation("LedgerEntry.MismatchedEntryId", "All posting lines must belong to this ledger entry."));
            }

            decimal totalDebits = postingLines
                .Where(x => x.Direction == PostingDirection.Debit)
                .Sum(x => x.Amount.Amount);

            decimal totalCredits = postingLines
                .Where(x => x.Direction == PostingDirection.Credit)
                .Sum(x => x.Amount.Amount);

            if(totalDebits != totalCredits)
            {
                return Result.Failure<LedgerEntry>(Error.Validation("LedgerEntry.Unbalanced", "Total debits must equal total credits."));
            }

            return Result.Success(new LedgerEntry(
                id,
                tenantId,
                transactionReference.Trim(),
                operationType.Trim(),
                description?.Trim() ?? string.Empty,
                postingLines.ToList()));
        }
    }
}
