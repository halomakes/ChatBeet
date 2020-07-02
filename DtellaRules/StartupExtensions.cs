using ChatBeet;
using DtellaRules.Rules;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DtellaRules
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddDtellaRules(this IServiceCollection services, IConfigurationSection adminConfigSection)
        {
            services.AddSingleton<IMessageRule, AstolfoRule>();

            services.Configure<DtellaRuleConfiguration>(c => adminConfigSection.Bind(c));

            return services;
        }
    }
}
