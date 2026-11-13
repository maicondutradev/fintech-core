using System;
using System.Collections.Generic;
using System.Text;

namespace FintechCore.Domain.Common
{
    public enum ErrorType
    {
        None = 0,
        Validation = 1,
        NotFound = 2,
        Conflict = 3,
        Failure = 4
    }
}
