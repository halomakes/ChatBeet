namespace NetIRC
{
    public class IRCPrefix
    {
        public string Raw { get; }
        public string From { get; }
        public string User { get; }
        public string Host { get; }

        public IRCPrefix(string prefixData)
        {
            Raw = prefixData;
            From = prefixData;

            if (prefixData.Contains("@"))
            {
                var splitedPrefix = prefixData.Split('@');
                From = splitedPrefix[0];
                Host = splitedPrefix[1];
            }

            if (From.Contains("!"))
            {
                var splittedFrom = From.Split('!');
                From = splittedFrom[0];
                User = splittedFrom[1];
            }
        }

        public override string ToString()
        {
            return Raw;
        }
    }
}
