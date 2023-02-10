using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatBeet.Services;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using SentimentAnalysis;

namespace ChatBeet.Commands;

[SlashModuleLifespan(SlashModuleLifespan.Scoped)]
public class SentimentCommandModule : ApplicationCommandModule
    {
    private readonly NegativeResponseService _negativeResponseService;
    private readonly DiscordClient _client;

    public SentimentCommandModule(NegativeResponseService negativeResponseService, DiscordClient client)
    {
        _negativeResponseService = negativeResponseService;
        _client = client;
    }

    [ContextMenu(ApplicationCommandType.MessageContextMenu, "Analyze Sentiment")]
    public async Task Kern(ContextMenuContext ctx)
    {
        if (ctx.TargetMessage.Author.Equals(_client.CurrentUser))
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                .WithContent(_negativeResponseService.GetResponseString()));
            return;
        }

        var data = new SentimentInput()
        {
            Message = ctx.TargetMessage.Content
        };
        var predictionResult = SentimentModel.Predict(data);
        var positiveScore = predictionResult.Score.LastOrDefault();
        var rating = Ratings.OrderBy(s => Math.Abs(s.rating - positiveScore)).FirstOrDefault();

        var isPositive = double.TryParse(predictionResult.Prediction, out var r) && r > 0.5;
        var scores = predictionResult.Score
            .Select(s => (fscore: s, rank: Convert.ToInt32(100 - (Math.Abs(1F - s) * 100))))
            .Select(pair => pair.fscore.ToString("F"));
        var rank = scores.LastOrDefault();
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
            .WithContent($"{Formatter.Timestamp(ctx.TargetMessage.Timestamp)}, {Formatter.Mention(ctx.TargetMessage.Author)} was {Formatter.Bold(rating.description)} {rating.emoji} (positive F₁ of {rank})"));
    }

    private static readonly List<(float rating, string emoji, string description)> Ratings = new()
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
}
