using FintechCore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FintechCore.Domain.Repositories
{
    public interface IAccountRepository
    {
        Task<Account?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Account?> GetByIdWithLockAsync (Guid id, CancellationToken cancellationToken = default);
        Task<bool> ExistsByAccountNumberAsync(string accountNumber, CancellationToken cancellationToken = default);
        void Add(Account account);
        void Update(Account account);
    }
}
