using ChatBeet;
using DtellaRules.Data;
using DtellaRules.Rules;
using DtellaRules.Services;
using IF.Lastfm.Core.Api;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Miki.Anilist;
using PixivCS;

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
            services.AddTransient<IMessageRule, MemoryCellRule>();
            services.AddTransient<IMessageRule, KerningRule>();
            services.AddTransient<IMessageRule, MockingTextRule>();
            services.AddTransient<IMessageRule, WaifuRule>();
            services.AddTransient<IMessageRule, AnimeRule>();
            services.AddTransient<IMessageRule, DeviantartRule>();
            services.AddTransient<IMessageRule, PixivRule>();
            services.AddTransient<IMessageRule, DadJokeRule>();

            services.AddTransient<DadJokeService>();
            services.AddTransient<PixivAppAPI>();
            services.AddTransient<DeviantartService>();
            services.AddTransient<AnilistClient>();
            services.AddTransient<AnilistService>();
            services.Configure<DtellaRuleConfiguration>(c => adminConfigSection.Bind(c));
            services.AddTransient<RecentTweetsService>();
            services.AddTransient(provider =>
            {
                var config = provider.GetService<IOptions<DtellaRuleConfiguration>>().Value.LastFm;
                return new LastfmClient(config.ClientId, config.ClientSecret);
            });
            services.AddTransient<LastFmService>();
            services.AddDbContext<DtellaContext>(ServiceLifetime.Transient);

            services.AddMemoryCache();

            return services;
        }
    }
}
