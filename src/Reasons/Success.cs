namespace Ultimately.Reasons
{
    using System.Collections.Generic;
    using System.Linq;


    public class Success : Reason
    {
        public static Success Default = new Success("");


        private Success(string message)
        {
            Message = message;
        }

        public static Success Create(string message)
        {
            return new Success(message);
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
                .WithInfoNoQuotes(nameof(Metadata), string.Join("; ", Metadata.Select(kvp => $"'{kvp.Key}: {kvp.Value}'")));
        }
    }
}
