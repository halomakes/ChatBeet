namespace ChatBeet.Models
{
    public struct Stat<TItem>
    {
        public TItem Item { get; set; }
        public int Count { get; set; }
    }
}
