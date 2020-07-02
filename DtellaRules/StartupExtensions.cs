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
            services.AddTransient<IMessageRule, StopRule>();
            services.AddTransient<IMessageRule, RecentTweetRule>();
            services.AddTransient<IMessageRule, AutoYatoRule>();

            services.Configure<DtellaRuleConfiguration>(c => adminConfigSection.Bind(c));
            services.AddTransient<RecentTweetsService>();

            return services;
        }
    }
}
