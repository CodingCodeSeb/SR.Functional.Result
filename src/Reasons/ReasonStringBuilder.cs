namespace Ultimately.Reasons
{
    using System;
    using System.Collections.Generic;
    using System.Linq;


    public class ReasonStringBuilder
    {
        private  string _reasonType = string.Empty;
        private readonly List<string> _infoStrings = new();

        public ReasonStringBuilder WithReasonType(Type type)
        {
            _reasonType = type.Name;

            return this;
        }

        public ReasonStringBuilder WithInfo(string label, string value)
        {
            var infoString = ToLabelValueStringOrEmpty(label, value);

            if (!string.IsNullOrEmpty(infoString))
            {
                _infoStrings.Add(infoString);
            }

            return this;
        }

        public ReasonStringBuilder WithInfoNoQuotes(string label, string value)
        {
            var infoString = ToLabelValueStringOrEmptyNoQuotes(label, value);

            if (!string.IsNullOrEmpty(infoString))
            {
                _infoStrings.Add(infoString);
            }

            return this;
        }

        public string Build()
        {
            var reasonInfoText = _infoStrings.Any()
                ? ReasonInfosToString(_infoStrings)
                : string.Empty;

            return $"{_reasonType}{(_reasonType != "" ? "=" : "")}{reasonInfoText}";
        }

        private static string ReasonInfosToString(IEnumerable<string> infoStrings)
        {
            return string.Join(", ", infoStrings);
        }

        private static string ToLabelValueStringOrEmpty(string label, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            return $"{(label != "" ? $"{label}=" : "")}'{value}'";
        }

        private static string ToLabelValueStringOrEmptyNoQuotes(string label, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            return $"{(label != "" ? $"{label}=" : "")}{value}";
        }
    }
}
