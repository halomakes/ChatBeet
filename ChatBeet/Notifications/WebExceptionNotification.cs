using System;
using MediatR;

namespace ChatBeet.Notifications;

public record WebExceptionNotification(Exception Exception) : INotification;