
namespace AccountHub.Application.Shared.ResultHelper
{
    public class Result
    {
        protected internal Result(bool isSuccess, IList<Error>? errors)
        {
            IsSuccess = isSuccess;
            Errors = errors;
        }

        public bool IsSuccess { get; }

        public bool IsFailure => !IsSuccess;

        public IList<Error>? Errors { get; }

        public static Result Success() => new(true, null);

        public static Result<TValue> Success<TValue>(TValue? value) => new(value, true, null);

        public static Result Failure(IList<Error>? errors) => new(false, errors);

        public static Result<TValue> Failure<TValue>(IList<Error>? errors) => new(default, false, errors);

        public static Result<TValue> Create<TValue>(TValue? value) => value is not null ? Success(value) : Failure<TValue>(null);
    }

    public class Result<TValue> : Result
    {
        private readonly TValue? _value;

        protected internal Result(TValue? value, bool isSuccess, IList<Error>? errors)
            : base(isSuccess, errors) =>
            _value = value;

        public TValue Value => IsSuccess
            ? _value!
            : throw new InvalidOperationException("The value of a failure result can not be accessed.");

        public static implicit operator Result<TValue>(TValue? value) => Create(value);
    }
}
