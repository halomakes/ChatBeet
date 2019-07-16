using ChatBeet.Queuing;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Options;
using ChatBeet.Queuing.Rules;
using NetIRC;
using NetIRC.Connection;
using NetIRC.Messages;

namespace ChatBeet.Irc
{
    internal class IrcBotService : IHostedService, IDisposable
    {
        private readonly Client client;
        private readonly IMessageQueueService queueService;
        private readonly ILogger<IrcBotService> logger;
        private readonly IrcBotConfiguration config;
        private Timer timer;

        public IrcBotService(IMessageQueueService queueService,
            ILogger<IrcBotService> logger,
            IOptions<IrcBotConfiguration> options)
        {
            this.queueService = queueService;
            this.logger = logger;
            config = options.Value;

            var user = new User(config.Nick, config.Identity);
            client = new Client(user, new TcpClientConnection());
            client.OnRawDataReceived += Client_OnRawDataReceived;
            client.EventHub.RegistrationCompleted += Client_OnRegistered;
            queueService.MessageAdded += QueueService_MessageAdded;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await Connect();
            timer = new Timer(ProcessQueue, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(100));
        }

        private async void QueueService_MessageAdded(object sender, EventArgs e)
        {
            await SendQueuedMessages();
        }

        private async void Client_OnRegistered(object sender, EventArgs e)
        {
            await JoinChannel(config.Channel);
        }

        private async void ProcessQueue(object state)
        {
            await SendQueuedMessages();
        }

        private async Task SendQueuedMessages()
        {
            await JoinChannel(config.Channel);

            var queue = queueService.PopAll();
            foreach (var q in queue)
            {
                await JoinChannel(q.Channel);
                await client.SendAsync(new NoticeMessage(q.Channel, q.Content));
            }
        }

        public async Task JoinChannel(string channelName)
        {
            if (!client.Channels.Any(c => c.Name == channelName))
                await client.SendAsync(new JoinMessage(channelName));
        }

        public async Task Connect()
        {
            await client.ConnectAsync(config.Server, config.Port);
            await client.SendAsync(new NickMessage(config.Nick));
            await client.SendAsync(new UserMessage(config.Nick, config.Identity));
        }

        private void Client_OnRawDataReceived(Client client, string rawData)
        {
            logger.LogInformation(rawData);
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
