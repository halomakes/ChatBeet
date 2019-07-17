using System;

namespace NetIRC
{
    /// <summary>
    /// Represents a chat message
    /// </summary>
    public class ChatMessage : EventArgs
    {
        public User User { get; }
        public string Text { get; }
        public DateTime Date { get; }

        public ChatMessage(User user, string text)
        {
            User = user;
            Text = text;
            Date = DateTime.Now;
        }
    }
}
