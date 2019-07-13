using ChatBeet.Queuing;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetIRC;
using NetIRC.Connection;
using NetIRC.Messages;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ChatBeet.Irc
{
    public class IrcBotService : IHostedService, IDisposable
    {
        private readonly Client client;
        private readonly IMessageQueueService queueService;
        private readonly ILogger<IrcBotService> logger;
        private Timer timer;

        public IrcBotService(IMessageQueueService queueService, ILogger<IrcBotService> logger)
        {
            this.queueService = queueService;
            this.logger = logger;
            var user = new User("ChatBeet", "A chat bot, but it's a root vegetable.");
            client = new Client(user, new TcpClientConnection());
            client.OnRawDataReceived += new IRCRawDataHandler((Client c, string msg) => logger.LogInformation(msg));
            client.EventHub.RegistrationCompleted += new EventHandler(async (o, a) => await Hub_OnRegistrationCompleted());
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await Connect();
            timer = new Timer(ProcessQueue, null, TimeSpan.Zero, TimeSpan.FromSeconds(15));
        }

        private void ProcessQueue(object state)
        {
            _ = Task.Run(() => SendQueuedMessages());
            logger.LogInformation("trigger");
        }

        private async Task Hub_OnRegistrationCompleted()
        {
            await JoinChannel();
        }

        private async void SendQueuedMessages()
        {
            var channels = client.Channels;
            await JoinChannel();
            //await client.SendRaw("messages! 1\r\n");
        }

        public async Task JoinChannel()
        {
            await client.SendAsync(new JoinMessage("#carrots"));
        }

        public async Task Connect()
        {
            await client.ConnectAsync("irc.dtella.net");
            await client.SendAsync(new NickMessage("ChatBeet"));
            await client.SendAsync(new UserMessage("ChatBeet", "A chat bot, but it's a root vegetable."));
        }

        public void Dispose()
        {
            client?.Dispose();
            timer?.Dispose();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Dispose();
            timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
    }
}
