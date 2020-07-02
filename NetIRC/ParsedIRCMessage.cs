using System;
using System.Collections.Generic;
using System.Linq;

namespace NetIRC
{
    /// <summary>
    /// Represents a parsed IRC message, providing information about it
    /// </summary>
    public class ParsedIRCMessage
    {
        /// <summary>
        /// The raw message received from the server
        /// </summary>
        public string Raw { get; }

        private IRCPrefix prefix;
        private string command;
        private string[] parameters;
        private IRCCommand ircCommand = IRCCommand.UNKNOWN;
        private IRCNumericReply numericReply = IRCNumericReply.UNKNOWN;

        /// <summary>
        /// The prefix of the message
        /// </summary>
        public IRCPrefix Prefix => prefix;

        /// <summary>
        /// The command received
        /// </summary>
        public string Command => command;

        /// <summary>
        /// Provides all parameters received in the message
        /// </summary>
        public string[] Parameters => parameters;

        /// <summary>
        /// Represents the last parameters in the message
        /// </summary>
        public string Trailing => parameters != null ? parameters[parameters.Length - 1] : string.Empty;

        /// <summary>
        /// An Enum representing the IRC command
        /// </summary>
        public IRCCommand IRCCommand => ircCommand;

        /// <summary>
        /// An Enum representing the IRC numeric reply
        /// </summary>
        public IRCNumericReply NumericReply => numericReply;

        /// <summary>
        /// Provides you a way to quickly check if the message is a numeric reply
        /// </summary>
        public bool IsNumeric => numericReply != IRCNumericReply.UNKNOWN;

        /// <summary>
        /// Initializes a new instance of ParsedIRCMessage, parsing the raw data
        /// </summary>
        /// <param name="rawData">Raw data to be parsed</param>
        public ParsedIRCMessage(string rawData)
        {
            Raw = rawData;
            Parse(rawData);
            ParseIRCEnums();
        }

        private void ParseIRCEnums()
        {
            if (string.IsNullOrEmpty(command))
            {
                return;
            }

            if (IsNumericReply(command))
            {
                Enum.TryParse(command, out numericReply);
                if (IsNumericReply(numericReply.ToString()))
                {
                    numericReply = IRCNumericReply.UNKNOWN;
                }
            }
            else
            {
                Enum.TryParse(command, out ircCommand);
            }
        }

        private void Parse(string rawData)
        {
            var trailing = string.Empty;
            var indexOfNextSpace = 0;

            if (RawDataHasPrefix)
            {
                indexOfNextSpace = rawData.IndexOf(' ');
                var prefixData = rawData.Substring(1, indexOfNextSpace - 1);
                prefix = new IRCPrefix(prefixData);
                rawData = rawData.Substring(indexOfNextSpace + 1);
            }

            var indexOfTrailingStart = rawData.IndexOf(" :");
            if (indexOfTrailingStart > -1)
            {
                trailing = rawData.Substring(indexOfTrailingStart + 2);
                rawData = rawData.Substring(0, indexOfTrailingStart);
            }

            if (DataDoesNotContainSpaces(rawData))
            {
                command = rawData;

                if (!string.IsNullOrEmpty(trailing))
                {
                    this.parameters = new[] { trailing };
                }

                return;
            }

            indexOfNextSpace = rawData.IndexOf(' ');
            command = rawData.Remove(indexOfNextSpace);
            rawData = rawData.Substring(indexOfNextSpace + 1);

            var parameters = new List<string>(rawData.Split(' '));

            if (!string.IsNullOrEmpty(trailing))
            {
                parameters.Add(trailing);
            }

            this.parameters = parameters.ToArray();
        }

        private bool RawDataHasPrefix => Raw.StartsWith(":");

        private bool DataDoesNotContainSpaces(string data) => !data.Contains(" ");

        private bool IsNumericReply(string command) => command.Length == 3 && command.ToCharArray().All(char.IsDigit);

        /// <summary>
        /// Returns a string that represents the parsed IRC message
        /// </summary>
        /// <returns>String that represents the parsed IRC message</returns>
        public override string ToString()
        {
            var paramsDescription = parameters != null ? "{ " + string.Join(", ", parameters) + " }" : string.Empty;
            return $"Prefix: {prefix}, Command: {command}, Params: {paramsDescription}, Trailing: {Trailing}";
        }
    }
}
