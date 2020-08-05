using ChatBeet.Utilities;
using GravyBot;
using GravyIrc.Messages;
using Microsoft.Extensions.Options;
using SampleClassification.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChatBeet.Rules
{
    public class SentimentAnalysisRule : NickLookupRule
    {
        public SentimentAnalysisRule(MessageQueueService messageQueueService, IOptions<IrcBotConfiguration> options) : base(messageQueueService, options)
        {
            CommandName = "sentiment";
        }

        protected override IEnumerable<IClientMessage> Respond(PrivateMessage incomingMessage, string nick, PrivateMessage lookupMessage)
        {
            var data = new SentimentInput()
            {
                Message = lookupMessage.Message
            };
            var predictionResult = SentimentModel.Predict(data);
            var isPositive = double.TryParse(predictionResult.Prediction, out var r) && r > 0.5;
            var sentiment = isPositive ? "positive" : "negative";
            var scores = predictionResult.Score
                .Select(s => (fscore: s, rank: Convert.ToInt32(100 - (Math.Abs(1F - s) * 100))))
                .Select(pair => pair.fscore.ToString("F").Colorize(pair.rank));
            var rank = isPositive ? scores.LastOrDefault() : scores.FirstOrDefault();

            yield return new PrivateMessage(incomingMessage.GetResponseTarget(), $"{lookupMessage.From} was {IrcValues.BOLD}{sentiment}{IrcValues.RESET} (F₁ of {rank})");
        }
    }
}
