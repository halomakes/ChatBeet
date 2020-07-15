using NetIRC.Connection;
using NetIRC.Messages;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NetIRC
{
    /// <summary>
    /// The NetIRC IRC client
    /// </summary>
    public class Client : IDisposable
    {
        private readonly IConnection connection;

        private readonly string password;

        /// <summary>
        /// Represents the user used to connect to the server
        /// </summary>
        public User User { get; }

        /// <summary>
        /// An observable collection representing the channels we joined
        /// </summary>
        public ChannelCollection Channels { get; }

        /// <summary>
        /// An observable collection representing all queries (private chat)
        /// </summary>
        public QueryCollection Queries { get; }

        /// <summary>
        /// An observable collection representing all peers (users) the client knows about
        /// It can be channel users, or query users (private chat)
        /// </summary>
        public UserCollection Peers { get; }

        /// <summary>
        /// Indicates that we received raw data from the server and gives you access to the data
        /// </summary>
        public event IRCRawDataHandler OnRawDataReceived;

        /// <summary>
        /// Indicates that we have parsed the message and gives you a strong typed representation of it
        /// You get the prefix, command, parameters and some other goodies
        /// </summary>
        public event ParsedIRCMessageHandler OnIRCMessageParsed;

        /// <summary>
        /// Provides you a way to handle various IRC events like OnPing and OnPrivMsg
        /// </summary>
        public EventHub EventHub { get; }

        /// <summary>
        /// Initializes a new instance of the IRC client with a User and an IConnection implementation
        /// </summary>
        /// <param name="user">User who wishes to connect to the server</param>
        /// <param name="connection">IConnection implementation</param>
        public Client(User user, IConnection connection)
        {
            User = user;

            this.connection = connection;
            this.connection.DataReceived += Connection_DataReceived;

            Channels = new ChannelCollection();
            Queries = new QueryCollection();
            Peers = new UserCollection();

            EventHub = new EventHub(this);
            InitializeDefaultEventHubEvents();
        }

        public Client(User user, string password, IConnection connection)
            : this(user, connection)
        {
            this.password = password;
        }

        private void InitializeDefaultEventHubEvents()
        {
            EventHub.Ping += EventHub_Ping;
            EventHub.Join += EventHub_Join;
            EventHub.Part += EventHub_Part;
            EventHub.Quit += EventHub_Quit;
            EventHub.PrivMsg += EventHub_PrivMsg;
            EventHub.RplNamReply += EventHub_RplNamReply;
            EventHub.Nick += EventHub_Nick;
        }

        private void EventHub_Nick(Client client, IRCMessageEventArgs<NickMessage> e)
        {
            var user = Peers.GetUser(e.IRCMessage.OldNick);
            user.Nick = e.IRCMessage.NewNick;
        }

        private void EventHub_PrivMsg(Client client, IRCMessageEventArgs<PrivMsgMessage> e)
        {
            var user = Peers.GetUser(e.IRCMessage.From);
            var message = new ChatMessage(user, e.IRCMessage.Message);

            if (e.IRCMessage.IsChannelMessage)
            {
                var channel = Channels.GetChannel(e.IRCMessage.To);
                channel.Messages.Add(message);
            }
            else
            {
                var query = Queries.GetQuery(user);
                query.Messages.Add(message);
            }
        }

        private void EventHub_RplNamReply(Client client, IRCMessageEventArgs<RplNamReplyMessage> e)
        {
            var channel = Channels.GetChannel(e.IRCMessage.Channel);
            foreach (var nick in e.IRCMessage.Nicks)
            {
                var user = Peers.GetUser(nick.Key);
                if (!channel.Users.Any(u => u.User.Nick == nick.Key))
                {
                    channel.AddUser(user, nick.Value);
                }
            }
        }

        private void EventHub_Quit(Client client, IRCMessageEventArgs<QuitMessage> e)
        {
            foreach (var channel in Channels)
            {
                var user = channel.Users.FirstOrDefault(u => u.Nick == e.IRCMessage.Nick);
                if (user != null)
                {
                    channel.Users.Remove(user);
                }
            }
        }

        private void EventHub_Part(Client client, IRCMessageEventArgs<PartMessage> e)
        {
            var channel = Channels.GetChannel(e.IRCMessage.Channel);
            channel.RemoveUser(e.IRCMessage.Nick);
        }

        private void EventHub_Join(Client client, IRCMessageEventArgs<JoinMessage> e)
        {
            var channel = Channels.GetChannel(e.IRCMessage.Channel);
            if (e.IRCMessage.Nick != User.Nick)
            {
                var user = Peers.GetUser(e.IRCMessage.Nick);
                channel.AddUser(user, string.Empty);
            }
        }

        private async void EventHub_Ping(object sender, IRCMessageEventArgs<PingMessage> e)
        {
            await SendAsync(new PongMessage(e.IRCMessage.Target));
        }

        private void Connection_DataReceived(object sender, DataReceivedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.Data))
            {
                return;
            }

            var rawData = e.Data;

            OnRawDataReceived?.Invoke(this, e.Data);

            var parsedIRCMessage = new ParsedIRCMessage(rawData);

            OnIRCMessageParsed?.Invoke(this, parsedIRCMessage);

            var serverMessage = IRCMessage.Create(parsedIRCMessage);

            serverMessage?.TriggerEvent(EventHub);
        }

        /// <summary>
        /// Connects to the specified IRC server using the specified port number
        /// </summary>
        /// <param name="host">IRC server address</param>
        /// <param name="port">Port number</param>
        /// <returns>The task object representing the asynchronous operation</returns>
        public async Task ConnectAsync(string host, int port = 6667)
        {
            await connection.ConnectAsync(host, port);

            if (!string.IsNullOrWhiteSpace(password))
            {
                await SendAsync(new PassMessage(password));
            }
            await SendAsync(new NickMessage(User.Nick));
            await SendAsync(new UserMessage(User.Nick, User.RealName));
        }

        /// <summary>
        /// Allows you to send raw data the the IRC server
        /// </summary>
        /// <param name="rawData">The raw data to be sent</param>
        /// <returns>The task object representing the asynchronous operation</returns>
        public async Task SendRaw(string rawData)
        {
            await connection.SendAsync(rawData);
        }

        /// <summary>
        /// Allows you to send a strong typed client message to the IRC server
        /// </summary>
        /// <param name="message">An implementation of IClientMessage. Check NetIRC.Messages namespace</param>
        /// <returns>The task object representing the asynchronous operation</returns>
        public async Task SendAsync(IClientMessage message)
        {
            var content = message.ToString();
            Console.WriteLine($"> {content}");
            await connection.SendAsync(content);
        }

        /// <summary>
        /// Disposes the connection
        /// </summary>
        public void Dispose()
        {
            connection.Dispose();
        }
    }
}
