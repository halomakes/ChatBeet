using GravyIrc;
using GravyIrc.Connection;
using GravyIrc.Messages;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ChatBeet.Irc
{
    internal class IrcBotService : IHostedService, IDisposable
    {
        private readonly Client client;
        private readonly MessageQueueService queueService;
        private readonly IrcBotConfiguration config;
        private Timer timer;
        private bool isRegistered = false;
        private readonly BotRulePipeline pipeline;

        public IrcBotService(MessageQueueService queueService, IOptions<IrcBotConfiguration> options, BotRulePipeline pipeline)
        {
            this.queueService = queueService;
            this.pipeline = pipeline;
            config = options.Value;

            var user = new User(config.Nick, config.Identity);
            client = new Client(user, new TcpClientConnection());
            client.OnRawDataReceived += Client_OnRawDataReceived;
            client.EventHub.Subscribe<RplWelcomeMessage>(Client_OnRegistered);
            queueService.MessageAdded += QueueService_MessageAdded;

            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            var method = typeof(IrcBotService).GetMethod(nameof(IrcBotService.SubscribeToEvent), BindingFlags.NonPublic | BindingFlags.Instance);
            var eligibleTypes = pipeline.SubscribedTypes.Where(t => typeof(IServerMessage).IsAssignableFrom(t));

            foreach (var type in eligibleTypes)
            {
                var genericMethod = method.MakeGenericMethod(type);
                genericMethod.Invoke(this, new object[] { });
            }
        }

        private void SubscribeToEvent<TMessage>() where TMessage : GravyIrc.Messages.IrcMessage, IServerMessage
        {
            client.EventHub.Subscribe<TMessage>((client, args) => queueService.Push(args.IrcMessage));
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
            await client.SendAsync(new PrivateMessage("NickServ", $"identify {config.NickServ}"));
            await client.SendAsync(new UserModeMessage(config.Nick, "+B"));
            await JoinDefaultChannels();
            isRegistered = true;
        }

        private async void ProcessQueue(object state)
        {
            if (isRegistered)
                await SendQueuedMessages();
        }

        private async Task SendQueuedMessages()
        {
            await JoinDefaultChannels();

            var queue = queueService.ViewAll().ToList();
            foreach (var q in queue)
            {
                if (q.Target.StartsWith("#"))
                    await JoinChannel(q.Target);
                await client.SendAsync(GenerateMessage(q));
                queueService.Remove(q);
            }
        }

        private async Task JoinDefaultChannels()
        {
            foreach (var c in config.Channels)
                await JoinChannel(c);
        }

        private static IClientMessage GenerateMessage(OutboundIrcMessage message)
        {
            switch (message.OutputType)
            {
                case IrcMessageType.Announcement:
                    return new NoticeMessage(message.Target, message.Content);
                case IrcMessageType.Activity:
                    return new PrivateMessage(message.Target, $"/me {message.Content}");
                default:
                    return new PrivateMessage(message.Target, message.Content);
            }
        }

        private static IInboundMessage ConvertInboundIrcMessage(PrivateMessage message) => new IrcMessage
        {
            DateRecieved = DateTime.Now,
            Channel = message.To,
            Sender = message.From,
            Content = message.Message
        };

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
