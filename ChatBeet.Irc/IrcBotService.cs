using ChatBeet.Queuing;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NetIRC;
using NetIRC.Connection;
using NetIRC.Messages;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ChatBeet.Irc
{
    internal class IrcBotService : IHostedService, IDisposable
    {
        private readonly Client client;
        private readonly IMessageQueueService queueService;
        private readonly IrcBotConfiguration config;
        private Timer timer;
        private bool isRegistered = false;

        public IrcBotService(IMessageQueueService queueService,
            IOptions<IrcBotConfiguration> options)
        {
            this.queueService = queueService;
            config = options.Value;

            var user = new User(config.Nick, config.Identity);
            client = new Client(user, new TcpClientConnection());
            client.OnRawDataReceived += Client_OnRawDataReceived;
            client.EventHub.RegistrationCompleted += Client_OnRegistered;
            client.EventHub.PrivMsg += EventHub_PrivMsg;
            queueService.MessageAdded += QueueService_MessageAdded;
        }

        private void EventHub_PrivMsg(Client client, IRCMessageEventArgs<PrivMsgMessage> e)
        {
            try
            {
                queueService.Push(QueuedChatMessage.FromChannelMessage(e.IRCMessage));
            }
            catch (Exception) { }
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
            isRegistered = true;
        }

        private async void ProcessQueue(object state)
        {
            if (isRegistered)
                await SendQueuedMessages();
        }

        private async Task SendQueuedMessages()
        {
            await JoinChannel(config.Channel);

            var queue = queueService.ViewAll().ToList();
            foreach (var q in queue)
            {
                if (q.Target.StartsWith("#"))
                    await JoinChannel(q.Target);
                await client.SendAsync(GenerateMessage(q));
                queueService.Remove(q);
            }
        }

        private static IClientMessage GenerateMessage(OutputMessage q)
        {
            switch (q.OutputType)
            {
                case Queuing.Rules.OutputType.Announcement:
                    return new NoticeMessage(q.Target, q.Content);
                case Queuing.Rules.OutputType.Activity:
                    return new PrivMsgMessage(q.Target, $"/me {q.Content}");
                default:
                    return new PrivMsgMessage(q.Target, q.Content);
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
            Console.WriteLine(rawData);
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
