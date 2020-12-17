namespace Ultimately.Reasons
{
    using System;
    using System.Collections.Generic;
    using System.Linq;


    public class Success : Reason
    {
        public static Success Default => new("");

        public List<Success> Reasons { get; }


        private Success()
        {
            Reasons = new List<Success>();
        }

        private Success(string message) : this()
        {
            Message = message;
        }

        private Success(string message, Success antecededBy) : this(message)
        {
            Reasons.Add(antecededBy);
        }


        public static Success Create(string message)
        {
            return new(message);
        }

        public static Success Create(string message, Success antecededBy)
        {
            return new(message, antecededBy);
        }


        public Success AntecededBy(string message)
        {
            Reasons.Add(new Success(message));

            return this;
        }

        public Success AntecededBy(Success success)
        {
            Reasons.Add(success);

            return this;
        }

        public Success AntecededBy<TValue>(Option<TValue> some)
        {
            if (!some.HasValue)
            {
                throw new InvalidOperationException("The optional value cannot empty in order to access its success object");
            }

            some.MatchSome((_, s) => Reasons.Add(s));

            return this;
        }

        public Success AntecededBy(Option some)
        {
            if (!some.IsSuccessful)
            {
                throw new InvalidOperationException("The optional value cannot unsuccessful in order to access its success object");
            }

            some.MatchSome(s => Reasons.Add(s));

            return this;
        }


        public Success WithMetadata(string metadataName, object metadataValue)
        {
            Metadata.Add(metadataName, metadataValue);

            return this;
        }

        public Success WithMetadata(Dictionary<string, object> metadata)
        {
            foreach (var metadataItem in metadata)
            {
                Metadata.Add(metadataItem.Key, metadataItem.Value);
            }

            return this;
        }



        protected override ReasonStringBuilder GetReasonStringBuilder()
        {
            return new ReasonStringBuilder()
                .WithInfo("", Message)
                .WithInfoNoQuotes(nameof(Metadata), string.Join("; ", Metadata.Select(kvp => $"'{kvp.Key}: {kvp.Value}'")))
                .WithInfoNoQuotes("Anteceded by", $"{string.Join(" ⁎ ", Reasons)}");
        }
    }
}
