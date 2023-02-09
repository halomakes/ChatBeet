using System;
using MediatR;

namespace ChatBeet.Models;

public class DownloadCompleteMessage : INotification
{
    public string Name { get; set; }

    public string Source { get; set; }

    public string Sender => Source;

    public string Content => Name;

    public DateTime DateRecieved { get; set; } = DateTime.Now;
}