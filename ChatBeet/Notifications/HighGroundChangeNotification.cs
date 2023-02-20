using ChatBeet.Data.Entities;
using MediatR;

namespace ChatBeet.Notifications;

public record HighGroundChangeNotification(User? Previous, User? Current) : INotification;