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
                pipeline.RegisterRule<ReplacementSetRule, PrivateMessage>();
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

            services.AddScoped<DadJokeService>();
            services.AddScoped<PixivAppAPI>();
            services.AddScoped<DeviantartService>();
            services.AddScoped<AnilistClient>();
            services.AddScoped<AnilistService>();
            services.Configure<ChatBeetConfiguration>(Configuration.GetSection("Rules:Dtella"));
            services.AddScoped<TwitterService>();
            services.AddScoped<TenorGifService>();
            services.AddScoped<Gelbooru>();
            services.AddScoped(provider =>
            {
                var config = provider.GetService<IOptions<ChatBeetConfiguration>>().Value.LastFm;
                return new LastfmClient(config.ClientId, config.ClientSecret);
            });
            services.AddScoped<LastFmService>();
            services.AddSingleton(provider =>
            {
                var config = provider.GetRequiredService<IOptions<ChatBeetConfiguration>>().Value.Igdb;
                return IGDB.Client.Create(config.ApiKey);
            });
            services.AddScoped<BooruService>();
            services.AddScoped<UserPreferencesService>();
            services.AddScoped<KeywordService>();
            services.AddHttpContextAccessor();
            services.AddScoped<LogonService>();

            services.AddHostedService<ContextInitializer>();
            services.AddDbContext<MemoryCellContext>(opts => opts.UseSqlite("Data Source=db/memorycell.db"));
            services.AddDbContext<BooruContext>(opts => opts.UseSqlite("Data Source=db/booru.db"));
            services.AddDbContext<PreferencesContext>(opts => opts.UseSqlite("Data Source=db/userprefs.db"));
            services.AddDbContext<KeywordContext>(opts => opts.UseSqlite("Data Source=db/keywords.db"));
            services.AddDbContext<ReplacementContext>(opts => opts.UseSqlite("Data Source=db/replacements.db"));
            services.AddDbContext<IdentityDbContext>(opts => opts.UseSqlite("Data Source=db/identity.db"));

            services.AddMemoryCache();
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<IdentityDbContext>()
                .AddDefaultTokenProviders();
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = LogonService.Scheme;
                options.DefaultChallengeScheme = LogonService.Scheme;
                options.DefaultScheme = LogonService.Scheme;
            }).AddCookie(LogonService.Scheme);
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, MemoryCellContext mcDb, BooruContext bDb, IdentityDbContext ctx)
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
