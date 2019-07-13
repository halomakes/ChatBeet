using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatBeet.Irc
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddIrcBot(this IServiceCollection services)
        {
            services.AddHostedService<IrcBotService>();
            return services;
        }
    }
}
