namespace ChatBeet.Models
{
    public struct TagStat
    {
        public string Tag;
        public int Total;
        public StatMode Mode;

        public enum StatMode
        {
            User,
            Tag
        }
    }
}
