using DSharpPlus;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace ChatBeet.Services
{
    public class DiscordBotService : BackgroundService
    {
        private readonly DiscordClient _client;

        public DiscordBotService(DiscordClient client)
        {
            _client = client;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // just run forever
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await _client.ConnectAsync();
            _client.MessageCreated += async (s, e) =>
            {
                if (e.Message.Content.ToLower() == ".cb test")
                {
                    await e.Message.RespondAsync("it's working");
                }
            };
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
