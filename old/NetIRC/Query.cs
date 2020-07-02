using System.Collections.ObjectModel;

namespace NetIRC
{
    /// <summary>
    /// Represents a query (private chat).
    /// </summary>
    public class Query
    {
        public User User { get; }
        public string Nick => User.Nick;

        public ObservableCollection<ChatMessage> Messages { get; }

        public Query(User user)
        {
            User = user;
            Messages = new ObservableCollection<ChatMessage>();
        }
    }
}
