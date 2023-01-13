using ChatBeet.Commands.Discord;
using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.DependencyInjection;
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
        private readonly IServiceProvider _services;

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
            commands.SlashCommandErrored += async (e, x) => await LogError("Slash command failed", x.Exception);
            commands.AutocompleteErrored += async (e, x) => await LogError("Autocomplete failed", x.Exception);
            commands.ContextMenuErrored += async (e, x) => await LogError("Context menu failed", x.Exception);
            _client.ClientErrored += (e, x) =>
            {
                _logger.LogError(x.Exception, "Discord client error");
                return Task.CompletedTask;
            };
            _client.ModalSubmitted += OnModalSubmitted;

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
            commands.RegisterCommands<SystemInfoCommandModule>();
            commands.RegisterCommands<SuspicionCommandModule>();
            commands.RegisterCommands<MessageTransformCommandModule>();
            commands.RegisterCommands<ProgressCommandModule>();
            commands.RegisterCommands<SentimentCommandModule>();
            commands.RegisterCommands<IrcCommandModule>();
            commands.RegisterCommands<SauceCommandModule>();
            commands.RegisterCommands<PreferencesCommandModule>();
            commands.RegisterCommands<WhipCommandModule>();
            commands.RegisterCommands<GameLookupCommandModule>();
            commands.RegisterCommands<HighGroundCommandModule>();
            commands.RegisterCommands<JokeCommandModule>();
            commands.RegisterCommands<LastFmCommandModule>();
            await _client.ConnectAsync();
            await base.StartAsync(cancellationToken);
        }

        private async Task OnModalSubmitted(DiscordClient sender, ModalSubmitEventArgs e)
        {
            switch (e.Interaction.Data.CustomId)
            {
                case IrcCommandModule.VerifyModalId:
                    using (var scope = _services.CreateAsyncScope())
                    {
                        var service = scope.ServiceProvider.GetRequiredService<IrcMigrationService>();
                        await service.ValidateTokenAsync(e.Interaction, e.Values["nick"], e.Values["token"]);
                    }
                    break;
            }
        }

        private async Task LogError(string v, Exception exception)
        {
            _logger.LogError(exception, v);
            await _services.GetRequiredService<DiscordLogService>().LogError(v, exception);
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
