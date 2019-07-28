namespace Ultimately.Reasons
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;


    public class Error : Reason
    {
        public List<Error> Reasons { get; }

        private Error()
        {
            Reasons = new List<Error>();
        }

        protected Error(string message) : this()
        {
            Message = message;
        }

        protected Error(string message, Error causedBy) : this(message)
        {
            Reasons.Add(causedBy);
        }




        public static Error Create(string message)
        {
            return new Error(message);
        }

        public static Error Create(string message, Error causedBy)
        {
            return new Error(message, causedBy);
        }




        public Error CausedBy(string message)
        {
            Reasons.Add(new Error(message));

            return this;
        }

        public Error CausedBy(Error error)
        {
            Reasons.Add(error);

            return this;
        }

        public Error CausedBy<TValue>(Option<TValue> none)
        {
            if (none.HasValue)
            {
                throw new InvalidOperationException("The optional value must be empty in order to access its error object");
            }

            none.MatchNone(e => Reasons.Add(e));

            return this;
        }

        public Error CausedBy(Exception exception)
        {
            Reasons.Add(ExceptionalError.Create(exception));

            return this;
        }

        public Error CausedBy(string message, Exception exception)
        {
            Reasons.Add(ExceptionalError.Create(message, exception));

            return this;
        }


        public Error WithMetadata(string metadataName, object metadataValue)
        {
            Metadata.Add(metadataName, metadataValue);

            return this;
        }

        public Error WithMetadata(KeyValuePair<string, object> metadataEntry)
        {
            Metadata.Add(metadataEntry.Key, metadataEntry.Value);

            return this;
        }

        public Error WithMetadata(Dictionary<string, object> metadata)
        {
            foreach (var metadataItem in metadata)
            {
                Metadata.Add(metadataItem.Key, metadataItem.Value);
            }

            return this;
        }

        public Error WithCallerMethodMetadata([CallerFilePath] string callingClassFilepath = "", [CallerMemberName] string callingMethod = "", [CallerLineNumber] int callingClassLineNumber = 0)
        {
            Metadata.Add(callingClassFilepath, $"{callingMethod}:{callingClassLineNumber}");

            return this;
        }


        public static implicit operator Error(string message)
        {
            return Create(message);
        }

        /// <summary>
        /// Formats the error object as a one-line string.
        /// </summary>
        public string Print(string separator = " → ")
        {
            return string.Join(separator, new List<string> { Message }.Concat(Reasons.Select(r => r.Message)));
        }


        protected override ReasonStringBuilder GetReasonStringBuilder()
        {
            return new ReasonStringBuilder()
                .WithInfo("", Message)
                .WithInfoNoQuotes(nameof(Metadata), string.Join("; ", Metadata.Select(kvp => $"{kvp.Key}: {kvp.Value}")))
                .WithInfoNoQuotes("Caused by", $"{string.Join(" ⁎ ", Reasons)}");
        }
    }
}
