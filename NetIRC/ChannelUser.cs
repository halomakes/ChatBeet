namespace NetIRC
{
    /// <summary>
    /// Represents a user in a specific channel
    /// </summary>
    public class ChannelUser
    {
        public User User { get; }
        public string Status { get; }

        public string Nick => User.Nick;

        public ChannelUser(User user, string status)
        {
            User = user;
            Status = status;
        }

        public override string ToString()
        {
            return Status + User.Nick;
        }
    }
}
