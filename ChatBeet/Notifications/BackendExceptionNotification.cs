using System;
using MediatR;

namespace ChatBeet.Notifications;

public record BackendExceptionNotification(Exception Exception) : INotification;