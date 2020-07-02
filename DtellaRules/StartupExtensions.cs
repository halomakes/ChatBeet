using ChatBeet;
using DtellaRules.Rules;
using DtellaRules.Services;
using IF.Lastfm.Core.Api;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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
            services.AddTransient<IMessageRule, ArtistRule>();
            services.AddTransient<IMessageRule, TrackRule>();

            services.Configure<DtellaRuleConfiguration>(c => adminConfigSection.Bind(c));
            services.AddTransient<RecentTweetsService>();
            services.AddTransient(provider =>
            {
                var config = provider.GetService<IOptions<DtellaRuleConfiguration>>().Value.LastFm;
                return new LastfmClient(config.ClientId, config.ClientSecret);
            });
            services.AddTransient<LastFmService>();

            return services;
        }
    }
}
