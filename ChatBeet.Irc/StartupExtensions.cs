using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ChatBeet.Irc
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddIrcBot(this IServiceCollection services, IConfigurationSection configSection)
        {
            services.Configure<IrcBotConfiguration>(c => configSection.Bind(c));

            services.AddHostedService<IrcBotService>();
            return services;
        }

        public static IServiceCollection AddIrcBot(this IServiceCollection services, Action<IrcBotConfiguration> configure)
        {
            if (configure == null)
            {
                configure = (options) => { };
            }

            services.Configure(configure);
            services.AddHostedService<IrcBotService>();
            return services;
        }
    }
}
