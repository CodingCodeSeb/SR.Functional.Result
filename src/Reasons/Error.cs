namespace SR.Functional.Reasons
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;


    public class Error : Reason
    {
        public static Error Default => new("");


        public List<Error> Reasons { get; }

        private Error()
        {
            Reasons = new List<Error>();
        }

        protected Error(string message) : this()
        {
            Message = message;
        }

        private Error(string message, Error causedBy) : this(message)
        {
            Reasons.Add(causedBy);
        }


        public static Error Create(string message)
        {
            return new(message);
        }

        public static Error Create(string message, Error causedBy)
        {
            return new(message, causedBy);
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

        public Error CausedBy<TValue>(Result<TValue> none)
        {
            if (none.IsSuccess)
            {
                throw new InvalidOperationException("The result value must be empty in order to access its error object");
            }

            none.IfFail(e => Reasons.Add(e));

            return this;
        }

        public Error CausedBy(Result none)
        {
            if (none.IsSuccess)
            {
                throw new InvalidOperationException("The result's outcome must be unsuccessful in order to access its error object");
            }

            none.IfFail(e => Reasons.Add(e));

            return this;
        }

        public Error CausedBy(Exception exception)
        {
            Reasons.Add(ExceptionalError.Create(exception));

            return this;
        }

        public Error WithMetadata(string metadataName, object metadataValue)
        {
            Metadata.Add(metadataName, metadataValue);

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
        /// Formats the error object as a one-line string, using the → symbol as a separator and an infinite depth.
        /// </summary>
        public string Print()
        {
            var errorMessageChain = GetErrorMessageChain(this);

            return string.Join(" → ", errorMessageChain);
        }

        /// <summary>
        /// Formats the error object as a one-line string, using the specified symbol as a separator and an infinite depth.
        /// </summary>
        /// <param name="separator">A string to delimit the indivudual error messages with.</param>
        public string Print(string separator)
        {
            var errorMessageChain = GetErrorMessageChain(this);

            return string.Join(separator, errorMessageChain);
        }

        /// <summary>
        /// Formats the error object as a one-line string, using the → symbol as a separator and using the specified depth value.
        /// </summary>
        /// <param name="depth">The number of levels to traverse in the error chain. Zero means infinite depth.</param>
        public string Print(byte depth)
        {
            var errorMessageChain = GetErrorMessageChain(this, depth);

            return string.Join(" → ", errorMessageChain);
        }

        /// <summary>
        /// Formats the error object as a one-line string using the specified transformation function on each individual message.
        /// </summary>
        /// <param name="transformFunc">A function to transform each message according to.</param>
        /// <param name="separator">A string to delimit the indivudual error messages with.</param>
        /// <param name="depth">The number of levels to traverse in the error chain. Zero means infinite depth.</param>
        public string Print(Func<string, string> transformFunc, string separator = " → ", byte depth = 0)
        {
            if (transformFunc == null)
            {
                transformFunc = s => s;
            }

            var errorMessageChain = GetErrorMessageChain(this, depth).Select(transformFunc);

            return string.Join(separator, errorMessageChain);
        }

        /// <summary>
        /// Formats the error object as a one-line string using the specified transformation function on each individual message.
        /// </summary>
        /// <param name="transformFunc">A function to transform each message according to. <para>The second argument specifies the zero-based index of the message in the error chain and the third argument is <see langword="true"/> when the message is the last one in the chain.</para></param>
        /// <param name="separator">A string to delimit the indivudual error messages with.</param>
        /// <param name="depth">The number of levels to traverse in the error chain. Zero means infinite depth.</param>
        public string Print(Func<string, int, bool, string> transformFunc, string separator = " → ", byte depth = 0)
        {
            if (transformFunc == null)
            {
                transformFunc = (m, _, __) => m;
            }

            var errorMessageChain = GetErrorMessageChain(this, depth).ToList();

            return string.Join(separator, errorMessageChain.Select((m, i) => transformFunc(m, i, i == errorMessageChain.Count - 1)));
        }

        private static IEnumerable<string> GetErrorMessageChain(Error error, byte depth = 0)
        {
            var currentDepth = 0;

            while (true)
            {
                if (error == null || depth > 0 && currentDepth == depth)
                {
                    yield break;
                }

                yield return error.Message;

                error = error.Reasons.FirstOrDefault();

                currentDepth++;
            }
        }


        protected override ReasonStringBuilder GetReasonStringBuilder()
        {
            return new ReasonStringBuilder().WithInfo("", Message)
                                            .WithInfoNoQuotes(nameof(Metadata), string.Join("; ", Metadata.Select(kvp => $"{kvp.Key}: {kvp.Value}")))
                                            .WithInfoNoQuotes("Caused by", $"{string.Join(" ⁎ ", Reasons)}");
        }
    }
}
