using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NetIRC
{
    /// <summary>
    /// Represents an IRC user. Implements INotifyPropertyChanged
    /// </summary>
    public class User : INotifyPropertyChanged
    {
        public User(string nick)
        {
            Nick = nick;
        }

        public User(string nick, string realName)
        {
            Nick = nick;
            RealName = realName;
        }

        private string nick;
        public string Nick
        {
            get { return nick; }
            set
            {
                if (nick != value)
                {
                    nick = value;
                    OnPropertyChanged();
                }
            }
        }
        public string RealName { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
