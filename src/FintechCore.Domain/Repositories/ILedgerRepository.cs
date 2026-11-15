using FintechCore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FintechCore.Domain.Repositories
{
    public interface ILedgerRepository
    {
        void Add(LedgerEntry ledgerEntry);
    }
}
