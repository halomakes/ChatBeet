using ChatBeet.Commands.Discord;
using DSharpPlus;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ChatBeet.Services
{
    public class DiscordBotService : BackgroundService
    {
        private readonly DiscordClient _client;
        private readonly ILogger<DiscordBotService> _logger;
        public readonly IServiceProvider _services;

        public DiscordBotService(DiscordClient client, ILogger<DiscordBotService> logger, IServiceProvider services)
        {
            _client = client;
            _logger = logger;
            _services = services;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // just run forever
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            var commands = _client.UseSlashCommands(new SlashCommandsConfiguration
            {
                Services = _services
            });
            commands.SlashCommandErrored += (e, x) =>
            {
                _logger.LogError(x.Exception, "Slash command failed");
                return Task.CompletedTask;
            };
            commands.AutocompleteErrored += (e, x) =>
            {
                _logger.LogError(x.Exception, "Autocomplete failed");
                return Task.CompletedTask;
            };

            commands.RegisterCommands<AnilistCommandModule>();
            commands.RegisterCommands<BadBotCommandModule>();
            commands.RegisterCommands<BeerCommandModule>();
            commands.RegisterCommands<BirthdayCommandModule>();
            commands.RegisterCommands<BonkCommandModule>();
            commands.RegisterCommands<BooruCommandModule>();
            commands.RegisterCommands<ComplimentCommandModule>();
            commands.RegisterCommands<DoggoCommandModule>();
            commands.RegisterCommands<GoogleCommandModule>();
            commands.RegisterCommands<MemoryCellCommandModule>();
            await _client.ConnectAsync();
            await base.StartAsync(cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _client.DisconnectAsync();
            await base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            _client.Dispose();
            base.Dispose();
        }
    }
}
