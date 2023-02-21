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

    app.UseWebOptimizer();
    app.UseStaticFiles();

    app.MapFallbackToFile("index.html");
}

void RegisterServices(WebApplicationBuilder builder)
{
    builder.Services.AddControllers()
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
    builder.Services.AddRazorPages();
    builder.Services.AddMemoryCache();
    builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
    builder.Services.AddHttpContextAccessor();

    builder.Services.Configure<ChatBeetConfiguration>(builder.Configuration.GetSection("Rules"));
    builder.Services.Configure<ForwardedHeadersOptions>(options => { options.ForwardedHeaders = ForwardedHeaders.All; });

    AddHttpClients(builder);
    AddAuthentication(builder);
    AddThirdPartyProviders(builder);
    AddInternalServices(builder);
    ConfigureDatabases(builder);
    AddDiscordBot(builder);
    ConfigureSwagger(builder);

    builder.Services.AddWebOptimizer(pipeline => { pipeline.CompileScssFiles(); });
}

void AddDiscordBot(WebApplicationBuilder builder)
{
    builder.Services.Configure<DiscordConfiguration>(c =>
    {
        c.Token = builder.Configuration.GetValue<string>("Discord:Token");
        c.TokenType = TokenType.Bot;
        c.Intents = DiscordIntents.MessageContents | DiscordIntents.Guilds | DiscordIntents.AllUnprivileged;
    });
    builder.Services.AddSingleton<DiscordClient>(ctx => new(ctx.GetRequiredService<IOptions<DiscordConfiguration>>().Value));
    builder.Services.AddHostedService<DiscordBotService>();
    builder.Services.Configure<DiscordBotConfiguration>(builder.Configuration.GetSection("Discord"));
    builder.Services.AddTransient<DiscordLogService>();
}

void ConfigureSwagger(WebApplicationBuilder builder)
{
    builder.Services.AddSwaggerGen(c =>
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

void ConfigureDatabases(WebApplicationBuilder builder)
{
    builder.Services.AddDbContext<CbDbContext>(opts => opts.UseNpgsql(builder.Configuration.GetConnectionString("ChatBeet")).UseSnakeCaseNamingConvention());

    builder.Services.AddScoped<IUsersRepository>(ctx => ctx.GetRequiredService<CbDbContext>());
    builder.Services.AddScoped<IBooruRepository>(ctx => ctx.GetRequiredService<CbDbContext>());
    builder.Services.AddScoped<IKeywordsRepository>(ctx => ctx.GetRequiredService<CbDbContext>());
    builder.Services.AddScoped<IDefinitionsRepository>(ctx => ctx.GetRequiredService<CbDbContext>());
    builder.Services.AddScoped<IProgressRepository>(ctx => ctx.GetRequiredService<CbDbContext>());
    builder.Services.AddScoped<IHighGroundRepository>(ctx => ctx.GetRequiredService<CbDbContext>());
    builder.Services.AddScoped<IKarmaRepository>(ctx => ctx.GetRequiredService<CbDbContext>());
    builder.Services.AddScoped<ISuspicionRepository>(ctx => ctx.GetRequiredService<CbDbContext>());
}

void AddAuthentication(WebApplicationBuilder builder)
{
    builder.Services.AddAuthentication(options => { options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme; }).AddCookie(options =>
        {
            options.LoginPath = "/Account/Login";
            options.LogoutPath = "/Account/Logout";
        })
        .AddDiscord(options =>
        {
            options.ClientSecret = builder.Configuration.GetValue<string>("Discord:ClientSecret")!;
            options.ClientId = builder.Configuration.GetValue<string>("Discord:ClientId")!;
            options.ClaimActions.MapCustomJson("urn:discord:avatar:url", user =>
                string.Format(
                    CultureInfo.InvariantCulture,
                    "https://cdn.discordapp.com/avatars/{0}/{1}.{2}",
                    user.GetString("id"),
                    user.GetString("avatar"),
                    user.GetString("avatar")!.StartsWith("a_") ? "gif" : "png"));
        });

    builder.Services.AddAuthorization(opts => { opts.AddPolicy(InGuildRequirement.Policy, policy => policy.Requirements.Add(new InGuildRequirement())); });
    builder.Services.AddScoped<IAuthorizationHandler, InGuildHandler>();
}

void AddInternalServices(WebApplicationBuilder builder)
{
    builder.Services.AddScoped<UserPreferencesService>();
    builder.Services.AddScoped<KeywordService>();
    builder.Services.AddScoped<NegativeResponseService>();
    builder.Services.AddScoped<GoogleSearchService>();
    builder.Services.AddScoped<LinkPreviewService>();
    builder.Services.AddScoped<SpeedometerService>();
    builder.Services.AddHostedService<ContextInitializer>();
    builder.Services.AddTransient<GraphicsService>();
    builder.Services.AddScoped<SuspicionService>();
    builder.Services.AddScoped<WebIdentityService>();
    builder.Services.AddScoped<GuildService>();
    builder.Services.AddScoped<KarmaService>();
    builder.Services.AddScoped<MustafarService>();
}

void AddHttpClients(WebApplicationBuilder builder)
{
    builder.Services.AddHttpClient();
    builder.Services.AddHttpClient("no-redirect").ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        AllowAutoRedirect = false
    });
    builder.Services.AddHttpClient("compression").ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        AutomaticDecompression = DecompressionMethods.All
    });
}

void AddThirdPartyProviders(WebApplicationBuilder builder)
{
    builder.Services.AddScoped<ComplimentService>();
    builder.Services.AddScoped<DadJokeService>();
    builder.Services.AddScoped<PixivApiClient>();
    builder.Services.AddScoped<DeviantartService>();
    builder.Services.AddScoped<AnilistClient>();
    builder.Services.AddScoped<AnilistService>();
    builder.Services.AddScoped<TwitterService>();
    builder.Services.AddScoped<TenorGifService>();
    builder.Services.AddScoped<Gelbooru>();
    builder.Services.AddScoped(provider =>
    {
        var config = provider.GetService<IOptions<ChatBeetConfiguration>>()!.Value.LastFm;
        return new LastfmClient(config.ClientId, config.ClientSecret);
    });
    builder.Services.AddScoped<LastFmService>();
    builder.Services.AddSingleton(provider =>
    {
        var config = provider.GetRequiredService<IOptions<ChatBeetConfiguration>>().Value.Igdb;
        return new IGDB.IGDBClient(config.ClientId, config.ClientSecret);
    });
    builder.Services.AddScoped<BooruService>();
    builder.Services.AddScoped<MemeService>();
    builder.Services.AddScoped<UrbanDictionaryApi>();
    builder.Services.AddScoped<YoutubeClient>();
    builder.Services.AddScoped(provider =>
    {
        var config = provider.GetService<IOptions<ChatBeetConfiguration>>();
        var opts = Options.Create(config!.Value.Untappd);
        var clientFactory = provider.GetService<IHttpClientFactory>();
        return new UntappdClient(clientFactory!.CreateClient(), opts);
    });
    builder.Services.AddScoped(provider =>
    {
        var config = provider.GetService<IOptions<ChatBeetConfiguration>>();
        return new SauceNETClient(config!.Value.Sauce);
    });
    builder.Services.AddScoped(_ => new OpenWeatherMapClient(builder.Configuration.GetValue<string>("Rules:OpenWeatherMap:ApiKey")));
    builder.Services.AddSingleton(_ => new WolframAlphaClient(builder.Configuration.GetValue<string>("Rules:Wolfram:AppId")));
    builder.Services.AddScoped(provider =>
    {
        var client = provider.GetRequiredService<IHttpClientFactory>().CreateClient();
        return new DogApi.DogApiClient(client, builder.Configuration.GetValue<string>("Rules:DogApi:ApiKey"));
    });
}