using ChatBeet.Configuration;
using DSharpPlus;
using DSharpPlus.Entities;
using Humanizer;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace ChatBeet.Services;
public class DiscordLogService
{
    private readonly DiscordClient _client;
    private readonly DiscordBotConfiguration _config;
    private static DiscordChannel logChannel;

    public DiscordLogService(DiscordClient client, IOptions<DiscordBotConfiguration> options)
    {
        _client = client;
        _config = options.Value;
    }

    public async Task LogError(string message, Exception exception)
    {
        if (logChannel is null)
        {
            try
            {
                logChannel = await _client.GetChannelAsync(_config.LogChannel);
            }
            catch { }
        }
        if (logChannel is not null)
        {
            try
            {
                if (exception is not null)
                {
                    var trace = $"Base {exception.GetType().Name} {exception.Message}";
                    var depth = 0;
                    var currentException = exception;

                    while (depth < 4 && currentException.InnerException != null)
                    {
                        currentException = currentException.InnerException;
                        depth++;
                        trace += $"{Environment.NewLine}Inner {currentException.GetType().Name} {currentException.Message}";
                    }
                    message += $"{Environment.NewLine}{Formatter.BlockCode(trace.Truncate(1950))}";
                }

                await _client.SendMessageAsync(logChannel, message);

                if (exception is not null)
                {
                    await _client.SendMessageAsync(logChannel, Formatter.BlockCode(exception.StackTrace.Truncate(1990)));
                }
            }
            catch (Exception e)
            {
                Console.Write(e);
            }
        }
    }
}
