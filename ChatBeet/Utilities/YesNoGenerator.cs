using System;
using System.Linq;

namespace ChatBeet.Utilities;
public class YesNoGenerator
{
    private static DateTime _lastIndeterminateResponse;
    private static readonly TimeSpan AntiAnnoyingWindow = TimeSpan.FromMinutes(10);
    private static readonly string[] PositiveResponses = new[] {
        "It is certain",
        "It is decidedly so",
        "Without a doubt",
        "Yes definitely",
        "You may rely on it",
        "As I see it, yes",
        "Most likely",
        "Outlook good",
        "Yes",
        "Signs point to yes"
    };
    private static readonly string[] IndeterminateResponses = new[]
    {
        "Reply hazy, try again",
        "Ask again later",
        "Better not tell you now",
        "Cannot predict now",
        "Concentrate and ask again"
    };
    private static readonly string[] NegativeResponses = new[]
    {
        "Don’t count on it",
        "My reply is no",
        "My sources say no",
        "Outlook not so good",
        "Very doubtful"
    };

    public static string GetResponse()
    {
        if (DateTime.Now - _lastIndeterminateResponse < AntiAnnoyingWindow)
        {
            var response = PositiveResponses
                    .Union(IndeterminateResponses)
                    .Union(NegativeResponses)
                    .PickRandom();
            if (IndeterminateResponses.Contains(response))
                _lastIndeterminateResponse = DateTime.Now;
            return response;
        }
        return PositiveResponses
            .Union(NegativeResponses)
            .PickRandom();
    }
}
