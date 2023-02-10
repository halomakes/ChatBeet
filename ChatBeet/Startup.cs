using BooruSharp.Booru;
using ChatBeet.Configuration;
using ChatBeet.Data;
using ChatBeet.Services;
using DSharpPlus;
using Genbox.WolframAlpha;
using IF.Lastfm.Core.Api;
using Meowtrix.PixivApi;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Miki.Anilist;
using Miki.UrbanDictionary;
using SauceNET;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Untappd.Client;
using YoutubeExplode;
using OpenWeatherMapClient = OpenWeatherMap.Standard.Current;

namespace ChatBeet;

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
        services.AddMemoryCache();
        services.AddMediatR(typeof(Startup));
        services.AddHttpContextAccessor();

        services.Configure<ChatBeetConfiguration>(Configuration.GetSection("Rules"));
        services.Configure<ForwardedHeadersOptions>(options => { options.ForwardedHeaders = ForwardedHeaders.All; });

        AddHttpClients(services);
        AddAuthentication(services);
        AddThirdPartyProviders(services);
        AddInternalServices(services);
        ConfigureDatabases(services);
        AddDiscordBot(services);
        ConfigureSwagger(services);

        services.AddWebOptimizer(pipeline => { pipeline.CompileScssFiles(); });
    }

    private void AddDiscordBot(IServiceCollection services)
    {
        services.Configure<DiscordConfiguration>(c =>
        {
            c.Token = Configuration.GetValue<string>("Discord:Token");
            c.TokenType = TokenType.Bot;
            c.Intents = DiscordIntents.MessageContents | DiscordIntents.AllUnprivileged;
        });
        services.AddSingleton<DiscordClient>(ctx => new(ctx.GetRequiredService<IOptions<DiscordConfiguration>>().Value));
        services.AddHostedService<DiscordBotService>();
        services.Configure<DiscordBotConfiguration>(Configuration.GetSection("Discord"));
        services.AddTransient<DiscordLogService>();
    }

    private static void ConfigureSwagger(IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "ChatBeet",
                Description = "A chat bot, but also a root vegetable."
            });
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
        });
    }

    private void ConfigureDatabases(IServiceCollection services)
    {
        services.AddDbContext<CbDbContext>(opts => opts.UseNpgsql(Configuration.GetConnectionString("ChatBeet")));

        services.AddScoped<IUsersRepository>(ctx => ctx.GetRequiredService<CbDbContext>());
        services.AddScoped<IBooruRepository>(ctx => ctx.GetRequiredService<CbDbContext>());
        services.AddScoped<IKeywordsRepository>(ctx => ctx.GetRequiredService<CbDbContext>());
        services.AddScoped<IDefinitionsRepository>(ctx => ctx.GetRequiredService<CbDbContext>());
        services.AddScoped<IProgressRepository>(ctx => ctx.GetRequiredService<CbDbContext>());
        services.AddScoped<IHighGroundRepository>(ctx => ctx.GetRequiredService<CbDbContext>());
        services.AddScoped<IKarmaRepository>(ctx => ctx.GetRequiredService<CbDbContext>());
        services.AddScoped<ISuspicionRepository>(ctx => ctx.GetRequiredService<CbDbContext>());
    }

    private void AddAuthentication(IServiceCollection services)
    {
        services.AddAuthentication(options => { options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme; }).AddCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
            })
            .AddDiscord(options =>
            {
                options.ClientSecret = Configuration.GetValue<string>("Discord:ClientSecret")!;
                options.ClientId = Configuration.GetValue<string>("Discord:ClientId")!;
                options.ClaimActions.MapCustomJson("urn:discord:avatar:url", user =>
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "https://cdn.discordapp.com/avatars/{0}/{1}.{2}",
                        user.GetString("id"),
                        user.GetString("avatar"),
                        user.GetString("avatar")!.StartsWith("a_") ? "gif" : "png"));
            });
    }

    private static void AddInternalServices(IServiceCollection services)
    {
        services.AddScoped<UserPreferencesService>();
        services.AddScoped<KeywordService>();
        services.AddScoped<NegativeResponseService>();
        services.AddScoped<GoogleSearchService>();
        services.AddScoped<LinkPreviewService>();
        services.AddScoped<SpeedometerService>();
        services.AddHostedService<ContextInitializer>();
        services.AddTransient<GraphicsService>();
        services.AddScoped<SuspicionService>();
        services.AddScoped<WebIdentityService>();
    }

    private static void AddHttpClients(IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddHttpClient("no-redirect").ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            AllowAutoRedirect = false
        });
        services.AddHttpClient("compression").ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.All
        });
    }

    private void AddThirdPartyProviders(IServiceCollection services)
    {
        services.AddScoped<ComplimentService>();
        services.AddScoped<DadJokeService>();
        services.AddScoped<PixivApiClient>();
        services.AddScoped<DeviantartService>();
        services.AddScoped<AnilistClient>();
        services.AddScoped<AnilistService>();
        services.AddScoped<TwitterService>();
        services.AddScoped<TenorGifService>();
        services.AddScoped<Gelbooru>();
        services.AddScoped(provider =>
        {
            var config = provider.GetService<IOptions<ChatBeetConfiguration>>()!.Value.LastFm;
            return new LastfmClient(config.ClientId, config.ClientSecret);
        });
        services.AddScoped<LastFmService>();
        services.AddSingleton(provider =>
        {
            var config = provider.GetRequiredService<IOptions<ChatBeetConfiguration>>().Value.Igdb;
            return new IGDB.IGDBClient(config.ClientId, config.ClientSecret);
        });
        services.AddScoped<BooruService>();
        services.AddScoped<MemeService>();
        services.AddScoped<UrbanDictionaryApi>();
        services.AddScoped<YoutubeClient>();
        services.AddScoped(provider =>
        {
            var config = provider.GetService<IOptions<ChatBeetConfiguration>>();
            var opts = Options.Create(config!.Value.Untappd);
            var clientFactory = provider.GetService<IHttpClientFactory>();
            return new UntappdClient(clientFactory!.CreateClient(), opts);
        });
        services.AddScoped(provider =>
        {
            var config = provider.GetService<IOptions<ChatBeetConfiguration>>();
            return new SauceNETClient(config!.Value.Sauce);
        });
        services.AddScoped(_ => new OpenWeatherMapClient(Configuration.GetValue<string>("Rules:OpenWeatherMap:ApiKey")));
        services.AddSingleton(_ => new WolframAlphaClient(Configuration.GetValue<string>("Rules:Wolfram:AppId")));
        services.AddScoped(provider =>
        {
            var client = provider.GetRequiredService<IHttpClientFactory>().CreateClient();
            return new DogApi.DogApiClient(client, Configuration.GetValue<string>("Rules:DogApi:ApiKey"));
        });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        app.UseForwardedHeaders();

        app.UseExceptionHandler("/Error");

        app.UseSwagger();
        app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "ChatBeet"); });

        var cookieOptions = new CookiePolicyOptions
        {
            MinimumSameSitePolicy = SameSiteMode.Lax
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

        app.UseWebOptimizer();
        app.UseStaticFiles();
    }
}