using System.Reflection;

namespace ChatBeet.Models;

public record StatusModel(AssemblyName Version, TimeSpan Uptime);