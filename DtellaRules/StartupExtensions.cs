using ChatBeet;
using DtellaRules.Rules;
using DtellaRules.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DtellaRules
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddDtellaRules(this IServiceCollection services, IConfigurationSection adminConfigSection)
        {
            services.AddTransient<IMessageRule, AstolfoRule>();

            services.Configure<DtellaRuleConfiguration>(c => adminConfigSection.Bind(c));
            services.AddTransient<TwitterImageService>();

            return services;
        }
    }
}
