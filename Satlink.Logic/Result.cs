using System;

namespace Satlink.Logic
{
    /// <summary>
    /// Represents an operation result.
    /// </summary>
    public class Result
    {
        protected Result(bool success, string error)
        {
            if (success && error != string.Empty)
                throw new InvalidOperationException();
            if (!success && error == string.Empty)
                throw new InvalidOperationException();
            Success = success;
            Error = error;
        }

        /// <summary>
        /// Gets a value indicating whether the operation succeeded.
        /// </summary>
        public bool Success { get; }

        /// <summary>
        /// Gets the error message when the operation fails.
        /// </summary>
        public string Error { get; }

        /// <summary>
        /// Gets a value indicating whether the operation failed.
        /// </summary>
        public bool IsFailure => !Success;

        /// <summary>
        /// Creates a failed result.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <returns>The failed result.</returns>
        public static Result Fail(string message)
        {
            return new Result(false, message);
        }

        /// <summary>
        /// Creates a failed typed result.
        /// </summary>
        /// <typeparam name="T">The value type.</typeparam>
        /// <param name="message">The error message.</param>
        /// <returns>The failed result.</returns>
        public static Result<T> Fail<T>(string message)
        {
            return new Result<T>(default, false, message);
        }

        /// <summary>
        /// Creates a successful result.
        /// </summary>
        /// <returns>The successful result.</returns>
        public static Result Ok()
        {
            return new Result(true, string.Empty);
        }

        /// <summary>
        /// Creates a successful typed result.
        /// </summary>
        /// <typeparam name="T">The value type.</typeparam>
        /// <param name="value">The result value.</param>
        /// <returns>The successful result.</returns>
        public static Result<T> Ok<T>(T value)
        {
            return new Result<T>(value, true, string.Empty);
        }
    }

    /// <summary>
    /// Represents an operation result with a value.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    public class Result<T> : Result
    {
        protected internal Result(T value, bool success, string error)
            : base(success, error)
        {
            Value = value;
        }

        /// <summary>
        /// Gets or sets the result value.
        /// </summary>
        public T Value { get; set; }
    }
}