using System;

namespace GameModules.UI.Results
{
    public class UIResult<T>
    {
        public bool IsSuccess { get; private set; }
        public T Value { get; private set; }
        public Exception Exception { get; private set; }

        private UIResult(bool isSuccess, T value, Exception exception)
        {
            IsSuccess = isSuccess;
            Value = value;
            Exception = exception;
        }

        public static UIResult<T> Success(T value)
        {
            return new UIResult<T>(true, value, null);
        }

        public static UIResult<T> Failure(Exception exception = null)
        {
            return new UIResult<T>(false, default(T), exception);
        }

        public static UIResult<T> Failure(T defaultValue, Exception exception = null)
        {
            return new UIResult<T>(false, defaultValue, exception);
        }
    }
}

