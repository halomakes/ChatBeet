using ChatBeet.Configuration;
using ChatBeet.Data;
using ChatBeet.Models;
using ChatBeet.Rules;
using ChatBeet.Services;
using GravyBot;
using GravyBot.DefaultRules;
using GravyIrc.Messages;
using IF.Lastfm.Core.Api;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Miki.Anilist;
using PixivCS;

namespace ChatBeet
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddIrcBot(Configuration.GetSection("Irc"), pipeline =>
            {
                pipeline.AddSampleRules();

                pipeline.RegisterAsyncRule<AstolfoRule, PrivateMessage>();
                pipeline.RegisterAsyncRule<RecentTweetRule, PrivateMessage>();
                pipeline.RegisterRule<AutoYatoRule, PrivateMessage>();
                pipeline.RegisterAsyncRule<ArtistRule, PrivateMessage>();
                pipeline.RegisterAsyncRule<TrackRule, PrivateMessage>();
                pipeline.RegisterAsyncRule<MemoryCellRule, PrivateMessage>();
                pipeline.RegisterRule<KerningRule, PrivateMessage>();
                pipeline.RegisterRule<MockingTextRule, PrivateMessage>();
                pipeline.RegisterRule<EmojifyRule, PrivateMessage>();
                pipeline.RegisterAsyncRule<WaifuRule, PrivateMessage>();
                pipeline.RegisterAsyncRule<AnimeRule, PrivateMessage>();
                pipeline.RegisterAsyncRule<DeviantartRule, PrivateMessage>();
                pipeline.RegisterAsyncRule<PixivRule, PrivateMessage>();
                pipeline.RegisterAsyncRule<DadJokeRule, PrivateMessage>();
                pipeline.RegisterRule<YearProgressRule, PrivateMessage>();
                pipeline.RegisterRule<DownloadCompleteRule, DownloadCompleteMessage>();
            });

            services.AddHttpClient();

            services.AddTransient<DadJokeService>();
            services.AddTransient<PixivAppAPI>();
            services.AddTransient<DeviantartService>();
            services.AddTransient<AnilistClient>();
            services.AddTransient<AnilistService>();
            services.Configure<DtellaRuleConfiguration>(Configuration.GetSection("Rules:Dtella"));
            services.AddTransient<RecentTweetsService>();
            services.AddTransient(provider =>
            {
                var config = provider.GetService<IOptions<DtellaRuleConfiguration>>().Value.LastFm;
                return new LastfmClient(config.ClientId, config.ClientSecret);
            });
            services.AddTransient<LastFmService>();
            services.AddDbContext<DtellaContext>(ServiceLifetime.Transient);

            services.AddMemoryCache();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, DtellaContext db)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            db.Database.EnsureCreated();
        }
    }
}
