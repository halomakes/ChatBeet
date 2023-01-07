using ChatBeet.Commands.Discord;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ChatBeet.Services
{
    public class DiscordBotService : BackgroundService
    {
        private readonly DiscordClient _client;
        private readonly IServiceProvider _services;

        public DiscordBotService(DiscordClient client, IServiceProvider services)
        {
            _client = client;
            _services = services;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // just run forever
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            var provider = new ServiceCollection();

            var commands = _client.UseCommandsNext(new CommandsNextConfiguration
            {
                StringPrefixes = new[] { ".cb " },
                Services = _services
            });
            commands.RegisterCommands<AnilistCommandModule>();
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
