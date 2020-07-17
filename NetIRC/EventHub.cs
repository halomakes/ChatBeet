using NetIRC.Messages;
using System;

namespace NetIRC
{
    public class EventHub
    {
        private readonly Client client;

        internal EventHub(Client client)
        {
            this.client = client;
        }

        /// <summary>
        /// Indicates that we are properly registered on the server
        /// It happens when the server sends Replies 001 to 004 to a user upon successful registration
        /// </summary>
        public event EventHandler RegistrationCompleted;
        internal void OnRegistrationCompleted()
        {
            RegistrationCompleted?.Invoke(client, EventArgs.Empty);
        }

        /// <summary>
        /// Indicates that we received a PING message from the server
        /// The client automatically sends a PONG message response
        /// </summary>
        public event IRCMessageEventHandler<PingMessage> Ping;
        internal void OnPing(IRCMessageEventArgs<PingMessage> e)
        {
            Ping?.Invoke(client, e);
        }

        /// <summary>
        /// Indicates that we received a PRIVMSG message and provides you a PrivMsgMessage object
        /// </summary>
        public event IRCMessageEventHandler<PrivMsgMessage> PrivMsg;
        internal void OnPrivMsg(IRCMessageEventArgs<PrivMsgMessage> e)
        {
            PrivMsg?.Invoke(client, e);
        }

        /// <summary>
        /// Indicates that we received a NOTICE message and provides you a NoticeMessage object
        /// </summary>
        public event IRCMessageEventHandler<NoticeMessage> Notice;
        internal void OnNotice(IRCMessageEventArgs<NoticeMessage> e)
        {
            Notice?.Invoke(client, e);
        }

        /// <summary>
        /// Indicates that some peer changed his Nickname
        /// </summary>
        public event IRCMessageEventHandler<NickMessage> Nick;
        internal void OnNick(IRCMessageEventArgs<NickMessage> e)
        {
            Nick?.Invoke(client, e);
        }

        /// <summary>
        /// Indicates that we received a 001 (RPL_WELCOME) numeric reply message
        /// </summary>
        public event IRCMessageEventHandler<RplWelcomeMessage> RplWelcome;
        internal void OnRplWelcome(IRCMessageEventArgs<RplWelcomeMessage> e)
        {
            RplWelcome?.Invoke(client, e);
            OnRegistrationCompleted();
        }

        /// <summary>
        /// Indicates that we received a 002 (RPL_YOURHOST) numeric reply message
        /// </summary>
        public event IRCMessageEventHandler<RplYourHostMessage> RplYourHost;
        internal void OnRplYourHost(IRCMessageEventArgs<RplYourHostMessage> e)
        {
            RplYourHost?.Invoke(client, e);
        }

        /// <summary>
        /// Indicates that we received a 003 (RPL_CREATED) numeric reply message
        /// </summary>
        public event IRCMessageEventHandler<RplCreatedMessage> RplCreated;
        internal void OnRplCreated(IRCMessageEventArgs<RplCreatedMessage> e)
        {
            RplCreated?.Invoke(client, e);
        }

        /// <summary>
        /// Indicates that we received a 004 (RPL_MYINFO) numeric reply message
        /// </summary>
        public event IRCMessageEventHandler<RplMyInfoMessage> RplMyInfo;
        internal void OnRplMyInfo(IRCMessageEventArgs<RplMyInfoMessage> e)
        {
            RplMyInfo?.Invoke(client, e);
        }

        /// <summary>
        /// Indicates that we received a 005 (RPL_ISUPPORT) numeric reply message
        /// </summary>
        public event IRCMessageEventHandler<RplISupportMessage> RplISupport;
        internal void OnRplISupport(IRCMessageEventArgs<RplISupportMessage> e)
        {
            RplISupport?.Invoke(client, e);
        }

        /// <summary>
        /// Indicates that some peer has joined a channel
        /// </summary>
        public event IRCMessageEventHandler<JoinMessage> Join;
        internal void OnJoin(IRCMessageEventArgs<JoinMessage> e)
        {
            Join?.Invoke(client, e);
        }

        /// <summary>
        /// Indicates that some peer has left a channel
        /// </summary>
        public event IRCMessageEventHandler<PartMessage> Part;
        internal void OnPart(IRCMessageEventArgs<PartMessage> e)
        {
            Part?.Invoke(client, e);
        }

        /// <summary>
        /// Indicates we received a 353 (RPL_NAMREPLY) numeric reply
        /// which contains the list of all users in a channel
        /// </summary>
        public event IRCMessageEventHandler<RplNamReplyMessage> RplNamReply;
        internal void OnRplNamReply(IRCMessageEventArgs<RplNamReplyMessage> e)
        {
            RplNamReply?.Invoke(client, e);
        }

        /// <summary>
        /// Indicates that some peer has quit the server
        /// </summary>
        public event IRCMessageEventHandler<QuitMessage> Quit;
        internal void OnQuit(IRCMessageEventArgs<QuitMessage> e)
        {
            Quit?.Invoke(client, e);
        }

        /// <summary>
        /// Indicates that the bot has been kicked from a channel
        /// </summary>
        public event IRCMessageEventHandler<KickMessage> Kick;
        internal void OnKick(IRCMessageEventArgs<KickMessage> e)
        {
            Kick?.Invoke(client, e);
        }
    }
}
