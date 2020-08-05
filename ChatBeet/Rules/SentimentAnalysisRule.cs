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
        private static readonly List<(float rating, string emoji, string description)> Ratings = new List<(float rating, string emoji, string description)>
        {
            (0.05F, "🤬", "extremely negative"),
            (0.15F, "😡", "very negative"),
            (0.25F, "😠", "negative"),
            (0.35F,"☹", "somewhat negative"),
            (0.45F, "🙁", "slightly negative"),
            (0.55F, "😐", "neutral"),
            (0.65F, "🙂", "slightly positive"),
            (0.75F, "☺", "positive"),
            (0.85F, "😀", "very positive"),
            (0.95F, "😁", "extremely positive")
        };

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
            var positiveScore = predictionResult.Score.LastOrDefault();
            var rating = Ratings.OrderBy(s => Math.Abs(s.rating - positiveScore)).FirstOrDefault();

            var isPositive = double.TryParse(predictionResult.Prediction, out var r) && r > 0.5;
            var scores = predictionResult.Score
                .Select(s => (fscore: s, rank: Convert.ToInt32(100 - (Math.Abs(1F - s) * 100))))
                .Select(pair => pair.fscore.ToString("F").Colorize(pair.rank));
            var rank = scores.LastOrDefault();

            yield return new PrivateMessage(incomingMessage.GetResponseTarget(), $"{lookupMessage.From} was {IrcValues.BOLD}{rating.description}{IrcValues.RESET} {rating.emoji} (Positive F₁ of {rank})");
        }
    }
}
