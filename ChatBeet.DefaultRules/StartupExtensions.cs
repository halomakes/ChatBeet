using ChatBeet.DefaultRules.Rules;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChatBeet.DefaultRules
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddDefaultRules(this IServiceCollection services, IConfigurationSection adminConfigSection)
        {
            services.AddSingleton<IMessageRule, HelloRule>();
            services.AddSingleton<IMessageRule, ExceptionLoggingRule>();

            services.Configure<DefaultRulesConfiguration>(c => adminConfigSection.Bind(c));

            return services;
        }
    }
}
