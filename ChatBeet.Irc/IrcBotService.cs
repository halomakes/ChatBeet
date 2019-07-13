using ChatBeet.Queuing;
using Meebey.SmartIrc4net;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace ChatBeet.Irc
{
    public class IrcBotService : IHostedService, IDisposable
    {
        private readonly IrcClient client = new IrcClient();
        private readonly IMessageQueueService queueService;
        private readonly ILogger<IrcBotService> logger;
        private Timer timer;
        const string channelName = "#🥕";

        public IrcBotService(IMessageQueueService queueService, ILogger<IrcBotService> logger)
        {
            this.queueService = queueService;
            this.logger = logger;

            Configure();
            Connect();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Connect();
            timer = new Timer(ProcessQueue, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
            return Task.CompletedTask;
        }

        private void Configure()
        {
            client.Encoding = Encoding.UTF8;
            client.SendDelay = 1000;
            client.ActiveChannelSyncing = true;
            client.OnRawMessage += Client_OnRawMessage;
            client.OnRegistered += Client_OnRegistered;
        }

        private void Client_OnRegistered(object sender, EventArgs e)
        {
            JoinChannel();
        }

        private void Client_OnRawMessage(object sender, IrcEventArgs e)
        {
            logger.LogInformation(e.Data.RawMessage);
        }

        private void ProcessQueue(object state)
        {
            SendQueuedMessages();
            logger.LogInformation("trigger");
        }

        private void SendQueuedMessages()
        {
            JoinChannel();

            var queue = queueService.PopAll();
            queue.ForEach(q => client.SendMessage(SendType.Notice, channelName, q.Title));

            client.ListenOnce();
        }

        public void JoinChannel()
        {
            if (!client.JoinedChannels.Cast<string>().Any(c => c.Contains("carrots")))
                client.RfcJoin(channelName);
        }

        public void Connect()
        {
            if (!client.IsConnected)
            {
                client.Connect("irc.dtella.net", 6667);
                client.Login("ChatBeet", "A chat bot, but it's a root vegetable.");
            }
            JoinChannel();
        }

        public void Dispose()
        {
            client?.Disconnect();
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
