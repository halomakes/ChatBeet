using MediatR;

namespace ChatBeet.Models;

public class DownloadCompleteMessage : INotification
{
    public required string Name { get; set; }

    public required string Source { get; set; }

    public string Sender => Source;

    public string Content => Name;

    public DateTime DateRecieved { get; set; } = DateTime.Now;
}