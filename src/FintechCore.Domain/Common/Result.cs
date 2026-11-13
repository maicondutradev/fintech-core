using System;
using System.Collections.Generic;
using System.Text;

namespace FintechCore.Domain.Common
{
    public class Result
    {
        public bool IsSucess { get; }
        public bool IsFailure => !IsSucess;
        public Error Error { get; }

        protected Result(bool isSucess, Error error)
        {
            
            if (isSucess && error != Error.None || !isSucess && error == Error.None)
            {
                throw new ArgumentException("Invalid error state", nameof(error));
            }
            IsSucess = isSucess;
            Error = error;
        }
        public static Result Success() => new(true, Error.None);
        public static Result Failure(Error error) => new(false, error ?? throw new ArgumentNullException(nameof(error)));
        public static Result<TValue> Success<TValue>(TValue value) => new(value, true, Error.None);
        public static Result<TValue> Failure<TValue>(Error error) => new(default, false, error);
    }
    public class Result<TValue> : Result
    {
        private readonly TValue? _value;
        protected internal Result(TValue? value, bool isSuccess, Error error)
            : base(isSuccess, error)
        {
            _value = value;
        }

        public TValue Value => IsSucess
            ? _value!
            : throw new InvalidOperationException("Cannot access the value of a failed resultd.");
    }
}
