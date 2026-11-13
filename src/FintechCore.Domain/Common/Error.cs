using System;
using System.Collections.Generic;
using System.Text;

namespace FintechCore.Domain.Common
{
    public sealed record Error (string Code, string Description, ErrorType Type)
    {
        public static readonly Error None = new(string.Empty, string.Empty, ErrorType.None);
        public static Error NotFound(string code, string description) => new Error(code, description, ErrorType.NotFound);
        public static Error Validation(string code, string description) => new Error(code, description, ErrorType.Validation);
        public static Error Conflict(string code, string description) => new Error(code, description, ErrorType.Conflict);
        public static Error Failure(string code, string description) => new Error(code, description, ErrorType.Failure);
    }
}
