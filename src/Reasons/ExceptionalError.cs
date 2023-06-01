namespace SR.Functional.Reasons
{
    using System;


    public sealed class ExceptionalError : Error
    {
        public Exception Exception { get; }


        private ExceptionalError(string message, Exception exception) : base(message)
        {
            Exception = exception;
        }

        public static ExceptionalError Create(Exception exception)
        {
            return new(exception.Message, exception);
        }

        protected override ReasonStringBuilder GetReasonStringBuilder()
        {
            return new ReasonStringBuilder().WithInfoNoQuotes("", $"{Exception.GetType().Name}: '{Message}'");
        }
    }
}
