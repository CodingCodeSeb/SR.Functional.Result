namespace Ultimately.Reasons
{
    using System;


    public class ExceptionalError : Error
    {
        public Exception Exception { get; }


        private ExceptionalError(string message, Exception exception) : base(message)
        {
            Exception = exception;
        }

        public static ExceptionalError Create(Exception exception)
        {
            return new ExceptionalError(exception.Message, exception);
        }

        public static ExceptionalError Create(string message, Exception exception)
        {
            return new ExceptionalError(message, exception);
        }

        protected override ReasonStringBuilder GetReasonStringBuilder()
        {
            return base.GetReasonStringBuilder()
                .WithInfo(nameof(Exception), Exception.ToString());
        }
    }
}
