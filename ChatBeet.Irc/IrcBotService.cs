using ChatBeet.Queuing;
using Meebey.SmartIrc4net;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Options;
using ChatBeet.Queuing.Rules;

namespace ChatBeet.Irc
{
    internal class IrcBotService : IHostedService, IDisposable
    {
        private readonly IrcClient client = new IrcClient();
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

            Configure();
            Connect();
            MonitorIncomingMessages();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Connect();
            timer = new Timer(ProcessQueue, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(100));
            return Task.CompletedTask;
        }

        private void MonitorIncomingMessages()
        {
            queueService.MessageAdded += QueueService_MessageAdded;
        }

        private void QueueService_MessageAdded(object sender, EventArgs e)
        {
            SendQueuedMessages();
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
        }

        private void SendQueuedMessages()
        {
            JoinChannel();

            var queue = queueService.PopAll();
            queue.ForEach(q => client.SendMessage(GetSendType(q), q.Channel, q.Content));
        }

        private SendType GetSendType(OutputMessage message)
        {
            switch (message.OutputType)
            {
                case OutputType.Activity: return SendType.Action;
                case OutputType.Announcement: return SendType.Notice;
                default: return SendType.Message;
            }
        }

        public void JoinChannel()
        {
            if (!client.JoinedChannels.Cast<string>().Any(c => c.Contains(config.Channel)))
                client.RfcJoin(config.Channel);
        }

        public void Connect()
        {
            if (!client.IsConnected)
            {
                client.Connect(config.Server, config.Port);
                client.Login(config.Nick, config.Identity);
            }
            JoinChannel();
            Task.Run(() => client.Listen());
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
