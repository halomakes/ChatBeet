using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ChatBeet.Smtp
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddSmtpListener(this IServiceCollection services, IConfigurationSection configSection)
        {
            services.Configure<SmtpListenerConfiguration>(c => configSection.Bind(c));

            services.AddHostedService<SmtpListenerService>();
            return services;
        }

        public static IServiceCollection AddIrcBot(this IServiceCollection services, Action<SmtpListenerConfiguration> configure)
        {
            if (configure == null)
            {
                configure = (options) => { };
            }

            services.Configure(configure);
            services.AddHostedService<SmtpListenerService>();
            return services;
        }
    }
}
