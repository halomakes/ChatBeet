using System;

namespace ChatBeet.Models;

public class LoginCompleteNotification
{
    public string Nick { get; set; }
    public DateTime Time { get; private set; } = DateTime.Now;
}