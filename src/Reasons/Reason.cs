namespace Ultimately.Reasons
{
    using System.Collections.Generic;


    // https://github.com/altmann/FluentResults
    public abstract class Reason
    {
        public string Message { get; protected set; }

        public Dictionary<string, object> Metadata { get; protected set; }

        protected Reason()
        {
            Metadata = new Dictionary<string, object>();
        }

        protected virtual ReasonStringBuilder GetReasonStringBuilder()
        {
            return new ReasonStringBuilder()
                .WithReasonType(GetType())
                .WithInfo("", Message)
                .WithInfo(nameof(Metadata), string.Join("; ", Metadata));
        }

        public override string ToString()
        {
            return GetReasonStringBuilder()
                .Build();
        }
    }
}
