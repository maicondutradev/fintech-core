using System;
using System.Collections.Generic;
using System.Text;

namespace FintechCore.Domain.Common
{
    public interface ITenantEntity
    {
        Guid TenantId { get; }
    }
}
