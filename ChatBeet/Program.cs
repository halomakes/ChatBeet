using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using BooruSharp.Booru;
using ChatBeet.Authorization;
using ChatBeet.Configuration;
using ChatBeet.Data;
using ChatBeet.Services;
using DSharpPlus;
using Genbox.WolframAlpha;
using IF.Lastfm.Core.Api;
using MediatR;
using Meowtrix.PixivApi;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
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
using Untappd.Client;
using YoutubeExplode;
using OpenWeatherMapClient = OpenWeatherMap.Standard.Current;

var builder = WebApplication.CreateBuilder(args);
RegisterServices(builder);
var app = builder.Build();
Configure(app);
app.Run();

static void Configure(WebApplication app)
{
    AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    app.UseForwardedHeaders();
    app.UseHttpsRedirection();

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

    app.UseStaticFiles();

    app.MapFallbackToFile("index.html");
}

void RegisterServices(WebApplicationBuilder b)
{
    b.Services.AddControllers()
        .AddJsonOptions(opts =>
        {
            opts.JsonSerializerOptions.TypeInfoResolver = new DefaultJsonTypeInfoResolver()
            {
                Modifiers =
                {
                    static t =>
                    {
                        if (t.Type == typeof(long) || t.Type == typeof(ulong))
                        {
                            t.NumberHandling = JsonNumberHandling.WriteAsString | JsonNumberHandling.AllowReadingFromString;
                        }
                    }
                }
            };
        });
    b.Services.AddRazorPages();
    b.Services.AddMemoryCache();
    b.Services.AddMediatR(Assembly.GetExecutingAssembly());
    b.Services.AddHttpContextAccessor();

    b.Services.Configure<ChatBeetConfiguration>(b.Configuration.GetSection("Rules"));
    b.Services.Configure<ForwardedHeadersOptions>(options => { options.ForwardedHeaders = ForwardedHeaders.All; });

    AddHttpClients(b);
    AddAuthentication(b);
    AddThirdPartyProviders(b);
    AddInternalServices(b);
    ConfigureDatabases(b);
    AddDiscordBot(b);
    ConfigureSwagger(b);
}

void AddDiscordBot(WebApplicationBuilder b)
{
    b.Services.Configure<DiscordConfiguration>(c =>
    {
        c.Token = b.Configuration.GetValue<string>("Discord:Token");
        c.TokenType = TokenType.Bot;
        c.Intents = DiscordIntents.MessageContents | DiscordIntents.Guilds | DiscordIntents.AllUnprivileged;
    });
    b.Services.AddSingleton<DiscordClient>(ctx => new(ctx.GetRequiredService<IOptions<DiscordConfiguration>>().Value));
    b.Services.AddHostedService<DiscordBotService>();
    b.Services.Configure<DiscordBotConfiguration>(b.Configuration.GetSection("Discord"));
    b.Services.AddTransient<DiscordLogService>();
}

void ConfigureSwagger(WebApplicationBuilder b)
{
    b.Services.AddSwaggerGen(c =>
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

void ConfigureDatabases(WebApplicationBuilder b)
{
    b.Services.AddDbContext<CbDbContext>(opts => opts.UseNpgsql(b.Configuration.GetConnectionString("ChatBeet")).UseSnakeCaseNamingConvention());

    b.Services.AddScoped<IUsersRepository>(ctx => ctx.GetRequiredService<CbDbContext>());
    b.Services.AddScoped<IBooruRepository>(ctx => ctx.GetRequiredService<CbDbContext>());
    b.Services.AddScoped<IDefinitionsRepository>(ctx => ctx.GetRequiredService<CbDbContext>());
    b.Services.AddScoped<IProgressRepository>(ctx => ctx.GetRequiredService<CbDbContext>());
    b.Services.AddScoped<IHighGroundRepository>(ctx => ctx.GetRequiredService<CbDbContext>());
    b.Services.AddScoped<IKarmaRepository>(ctx => ctx.GetRequiredService<CbDbContext>());
    b.Services.AddScoped<ISuspicionRepository>(ctx => ctx.GetRequiredService<CbDbContext>());
    b.Services.AddScoped<IStatsRepository>(ctx => ctx.GetRequiredService<CbDbContext>());
    b.Services.AddScoped<IQuoteRepository>(ctx => ctx.GetRequiredService<CbDbContext>());
}

void AddAuthentication(WebApplicationBuilder b)
{
    b.Services.AddAuthentication(options => { options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme; }).AddCookie(options =>
        {
            options.LoginPath = "/Account/Login";
            options.LogoutPath = "/Account/Logout";
        })
        .AddDiscord(options =>
        {
            options.ClientSecret = b.Configuration.GetValue<string>("Discord:ClientSecret")!;
            options.ClientId = b.Configuration.GetValue<string>("Discord:ClientId")!;
            options.ClaimActions.MapCustomJson("urn:discord:avatar:url", user =>
                string.Format(
                    CultureInfo.InvariantCulture,
                    "https://cdn.discordapp.com/avatars/{0}/{1}.{2}",
                    user.GetString("id"),
                    user.GetString("avatar"),
                    user.GetString("avatar")!.StartsWith("a_") ? "gif" : "png"));
        });

    b.Services.AddAuthorization(opts => { opts.AddPolicy(InGuildRequirement.Policy, policy => policy.Requirements.Add(new InGuildRequirement())); });
    b.Services.AddScoped<IAuthorizationHandler, InGuildHandler>();
}

void AddInternalServices(WebApplicationBuilder b)
{
    b.Services.AddScoped<UserPreferencesService>();
    b.Services.AddScoped<NegativeResponseService>();
    b.Services.AddScoped<GoogleSearchService>();
    b.Services.AddScoped<LinkPreviewService>();
    b.Services.AddScoped<SpeedometerService>();
    b.Services.AddHostedService<ContextInitializer>();
    b.Services.AddTransient<GraphicsService>();
    b.Services.AddScoped<SuspicionService>();
    b.Services.AddScoped<WebIdentityService>();
    b.Services.AddScoped<GuildService>();
    b.Services.AddScoped<KarmaService>();
    b.Services.AddScoped<MustafarService>();
}

void AddHttpClients(WebApplicationBuilder b)
{
    b.Services.AddHttpClient();
    b.Services.AddHttpClient("no-redirect").ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        AllowAutoRedirect = false
    });
    b.Services.AddHttpClient("compression").ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        AutomaticDecompression = DecompressionMethods.All
    });
}

void AddThirdPartyProviders(WebApplicationBuilder b)
{
    b.Services.AddScoped<DadJokeService>();
    b.Services.AddScoped<PixivApiClient>();
    b.Services.AddScoped<AnilistClient>();
    b.Services.AddScoped<AnilistService>();
    b.Services.AddScoped<Gelbooru>();
    b.Services.AddScoped(provider =>
    {
        var config = provider.GetService<IOptions<ChatBeetConfiguration>>()!.Value.LastFm;
        return new LastfmClient(config.ClientId, config.ClientSecret);
    });
    b.Services.AddScoped<LastFmService>();
    b.Services.AddSingleton(provider =>
    {
        var config = provider.GetRequiredService<IOptions<ChatBeetConfiguration>>().Value.Igdb;
        return new IGDB.IGDBClient(config.ClientId, config.ClientSecret);
    });
    b.Services.AddScoped<BooruService>();
    b.Services.AddScoped<MemeService>();
    b.Services.AddScoped<UrbanDictionaryApi>();
    b.Services.AddScoped<YoutubeClient>();
    b.Services.AddScoped(provider =>
    {
        var config = provider.GetService<IOptions<ChatBeetConfiguration>>();
        var opts = Options.Create(config!.Value.Untappd);
        var clientFactory = provider.GetService<IHttpClientFactory>();
        return new UntappdClient(clientFactory!.CreateClient(), opts);
    });
    b.Services.AddScoped(provider =>
    {
        var config = provider.GetService<IOptions<ChatBeetConfiguration>>();
        return new SauceNETClient(config!.Value.Sauce);
    });
    b.Services.AddScoped(_ => new OpenWeatherMapClient(b.Configuration.GetValue<string>("Rules:OpenWeatherMap:ApiKey")));
    b.Services.AddSingleton(_ => new WolframAlphaClient(b.Configuration.GetValue<string>("Rules:Wolfram:AppId")));
    b.Services.AddScoped(provider =>
    {
        var client = provider.GetRequiredService<IHttpClientFactory>().CreateClient();
        return new DogApi.DogApiClient(client, b.Configuration.GetValue<string>("Rules:DogApi:ApiKey"));
    });
}