using BooruSharp.Booru;
using ChatBeet.Configuration;
using ChatBeet.Data;
using ChatBeet.Models;
using ChatBeet.Rules;
using ChatBeet.Services;
using GravyBot;
using GravyBot.DefaultRules;
using GravyIrc.Messages;
using IF.Lastfm.Core.Api;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
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
            services.AddRazorPages();

            services.AddIrcBot(Configuration.GetSection("Irc"), pipeline =>
            {
                pipeline.AddSampleRules();

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
                pipeline.RegisterRule<ProgressRule, PrivateMessage>();
                pipeline.RegisterRule<DownloadCompleteRule, DownloadCompleteMessage>();
                pipeline.RegisterAsyncRule<GifSearchRule, PrivateMessage>();
                pipeline.RegisterRule<SentimentAnalysisRule, PrivateMessage>();
                pipeline.RegisterAsyncRule<BooruRule, PrivateMessage>();
                pipeline.RegisterRule<KarmaReactRule, PrivateMessage>();
                pipeline.RegisterRule<HighGroundRule, PrivateMessage>();
                pipeline.RegisterRule<HighGroundRule, HighGroundClaim>();
                pipeline.RegisterRule<SlotMachineRule, PrivateMessage>();
                pipeline.RegisterAsyncRule<GameRule, PrivateMessage>();
                pipeline.RegisterAsyncRule<TwitterUrlPreviewRule, PrivateMessage>();
                pipeline.RegisterAsyncRule<ShipReactRule, PrivateMessage>();
                pipeline.RegisterAsyncRule<BooruBlacklistRule, PrivateMessage>();
                pipeline.RegisterRule<DrawLotsRule, PrivateMessage>();
                pipeline.RegisterAsyncRule<UserPreferencesRule, PrivateMessage>();
                pipeline.RegisterAsyncRule<PronounRule, PrivateMessage>();
                pipeline.RegisterAsyncRule<WorkdayProgressRule, PrivateMessage>();
                pipeline.RegisterRule<LoginTokenRule, LoginTokenRequest>();
                pipeline.RegisterRule<LoginNotificationRule, LoginCompleteNotification>();
                pipeline.RegisterRule<BadBotReactRule, PrivateMessage>();
                pipeline.RegisterRule<DefUpdatedRule, DefinitionChange>();
                pipeline.RegisterRule<UserPreferencesRule, PreferenceChange>();
                pipeline.RegisterAsyncRule<BirthdaysRule, PrivateMessage>();
                pipeline.RegisterAsyncRule<KeywordRule, PrivateMessage>();
            });

            services.AddHttpClient();

            services.AddTransient<DadJokeService>();
            services.AddTransient<PixivAppAPI>();
            services.AddTransient<DeviantartService>();
            services.AddTransient<AnilistClient>();
            services.AddTransient<AnilistService>();
            services.Configure<ChatBeetConfiguration>(Configuration.GetSection("Rules:Dtella"));
            services.AddTransient<TwitterService>();
            services.AddTransient<TenorGifService>();
            services.AddTransient<Gelbooru>();
            services.AddTransient(provider =>
            {
                var config = provider.GetService<IOptions<ChatBeetConfiguration>>().Value.LastFm;
                return new LastfmClient(config.ClientId, config.ClientSecret);
            });
            services.AddTransient<LastFmService>();
            services.AddSingleton(provider =>
            {
                var config = provider.GetRequiredService<IOptions<ChatBeetConfiguration>>().Value.Igdb;
                return IGDB.Client.Create(config.ApiKey);
            });
            services.AddTransient<BooruService>();
            services.AddTransient<UserPreferencesService>();
            services.AddTransient<KeywordService>();
            services.AddDbContext<MemoryCellContext>(ServiceLifetime.Transient);
            services.AddDbContext<BooruContext>(ServiceLifetime.Transient);
            services.AddDbContext<PreferencesContext>(ServiceLifetime.Transient);
            services.AddDbContext<KeywordContext>(ServiceLifetime.Transient);
            services.AddDbContext<IdentityDbContext>(opts => opts.UseInMemoryDatabase(databaseName: "auth"));

            services.AddMemoryCache();
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<IdentityDbContext>()
                .AddDefaultTokenProviders();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, MemoryCellContext mcDb, BooruContext bDb)
        {
            app.UseForwardedHeaders();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            var cookieOptions = new CookiePolicyOptions
            {
                MinimumSameSitePolicy = SameSiteMode.Strict
            };
            app.UseCookiePolicy(cookieOptions);

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });

            app.UseStaticFiles();

            mcDb.Database.EnsureCreated();
            bDb.Database.EnsureCreated();
        }
    }
}
