namespace Application.Common
{
    public class Result
    {
        protected Result(bool isSuccess, string error)
        {
            if (isSuccess && error != "" ||
            !isSuccess && error == "")
            {
                throw new ArgumentException("Invalid error", nameof(error));
            }

            IsSuccess = isSuccess;
            Error = error;
        }

        public bool IsSuccess { get; }

        public bool IsFailure => !IsSuccess;

        public string Error { get; }

        public static Result Success() => new(true, "");

        public static Result Failure(string error) => new(false, error);

        public static Result<TValue> Success<TValue>(TValue value) => new(value, true, "");

        public static Result<TValue> Failure<TValue>(string error) => new(default, false, error);
    }

    public class Result<TValue> : Result
    {
        private readonly TValue? _value;

        protected internal Result(TValue? value, bool isSuccess, string error)
            : base(isSuccess, error)
        {
            _value = value;
        }

        public TValue Value => IsSuccess
            ? _value!
            : throw new InvalidOperationException("The value of a failure result can't be accessed");

        public static implicit operator Result<TValue>(TValue? value) =>
            value is not null ? Success(value) : Failure<TValue>("");
    }
}
