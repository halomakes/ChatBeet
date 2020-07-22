using ChatBeet.Irc;
using DtellaRules.Data;
using DtellaRules.Models;
using DtellaRules.Rules;
using DtellaRules.Services;
using GravyIrc.Messages;
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
        public static BotRulePipeline AddDtellaRules(this BotRulePipeline pipeline)
        {
            pipeline.RegisterRule<AstolfoRule, PrivateMessage>();
            pipeline.RegisterRule<StopRule, PrivateMessage>();
            pipeline.RegisterRule<RecentTweetRule, PrivateMessage>();
            pipeline.RegisterRule<AutoYatoRule, PrivateMessage>();
            pipeline.RegisterRule<ArtistRule, PrivateMessage>();
            pipeline.RegisterRule<TrackRule, PrivateMessage>();
            pipeline.RegisterRule<MemoryCellRule, PrivateMessage>();
            pipeline.RegisterRule<KerningRule, PrivateMessage>();
            pipeline.RegisterRule<MockingTextRule, PrivateMessage>();
            pipeline.RegisterRule<WaifuRule, PrivateMessage>();
            pipeline.RegisterRule<AnimeRule, PrivateMessage>();
            pipeline.RegisterRule<DeviantartRule, PrivateMessage>();
            pipeline.RegisterRule<PixivRule, PrivateMessage>();
            pipeline.RegisterRule<DadJokeRule, PrivateMessage>();
            pipeline.RegisterRule<YearProgressRule, PrivateMessage>();
            pipeline.RegisterRule<DownloadCompleteRule, DownloadCompleteMessage>();

            return pipeline;
        }

        public static IServiceCollection AddDtellaRules(this IServiceCollection services, IConfigurationSection adminConfigSection)
        {
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
