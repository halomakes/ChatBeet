using BooruSharp.Booru;
using ChatBeet.Configuration;
using ChatBeet.Data;
using ChatBeet.Models;
using ChatBeet.Rules;
using ChatBeet.Services;
using GravyBot;
using GravyBot.Commands;
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
using Microsoft.OpenApi.Models;
using Miki.Anilist;
using PixivCS;
using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using OpenWeatherMapClient = OpenWeatherMap.Standard.Current;

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

            var botConfig = new ChatBeetConfiguration();
            Configuration.Bind("Rules", botConfig);

            services.AddIrcBot(Configuration.GetSection("Irc"), pipeline =>
            {
                pipeline.AddSampleRules();

                pipeline.RegisterAsyncRule<RecentTweetRule, PrivateMessage>();
                pipeline.RegisterRule<AutoYatoRule, PrivateMessage>();
                pipeline.RegisterAsyncRule<RememberRule, PrivateMessage>();
                pipeline.RegisterRule<ReplacementSetRule, PrivateMessage>();
                pipeline.RegisterAsyncRule<DadJokeRule, PrivateMessage>();
                pipeline.RegisterRule<DownloadCompleteRule, DownloadCompleteMessage>();
                pipeline.RegisterRule<KarmaReactRule, PrivateMessage>();
                pipeline.RegisterAsyncRule<TwitterUrlPreviewRule, PrivateMessage>();
                pipeline.RegisterAsyncRule<UserPreferencesRule, PrivateMessage>();
                pipeline.RegisterRule<LoginTokenRule, LoginTokenRequest>();
                pipeline.RegisterRule<LoginNotificationRule, LoginCompleteNotification>();
                pipeline.RegisterRule<DefUpdatedRule, DefinitionChange>();
                pipeline.RegisterRule<UserPreferencesRule, PreferenceChange>();
                pipeline.RegisterAsyncRule<BirthdaysRule, PrivateMessage>();
                pipeline.RegisterAsyncRule<KeywordRule, PrivateMessage>();
                pipeline.RegisterRule<StackTraceRule, Exception>();
                pipeline.RegisterAsyncRule<InterrogativeRecallRule, PrivateMessage>();
                pipeline.RegisterAsyncRule<CommandingRecallRule, PrivateMessage>();
                pipeline.RegisterAsyncRule<CurrentWeatherRule, PrivateMessage>();
                pipeline.RegisterAsyncRule<SuspectRule, PrivateMessage>();
                pipeline.AddCommandOrchestrator();
            });

            services.AddCommandOrchestrator(builder =>
            {
                builder.RegisterProcessors(Assembly.GetExecutingAssembly());
                builder.RegisterProcessor<HelpCommandProcessor>();
                builder.AddChannelPolicy("NoMain", botConfig.Policies["NoMain"]);
            });

            services.AddHttpClient();
            services.AddHttpClient("noredirect").ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                AllowAutoRedirect = false
            });

            services.AddScoped<DadJokeService>();
            services.AddScoped<PixivAppAPI>();
            services.AddScoped<DeviantartService>();
            services.AddScoped<AnilistClient>();
            services.AddScoped<AnilistService>();
            services.Configure<ChatBeetConfiguration>(Configuration.GetSection("Rules"));
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
                return new IGDB.IGDBClient(config.ClientId, config.ClientSecret);
            });
            services.AddScoped<BooruService>();
            services.AddScoped<UserPreferencesService>();
            services.AddScoped<KeywordService>();
            services.AddHttpContextAccessor();
            services.AddScoped<LogonService>();
            services.AddScoped<NegativeResponseService>();
            services.AddScoped(provider => new OpenWeatherMapClient(Configuration.GetValue<string>("Rules:OpenWeatherMap:ApiKey")));

            services.AddHostedService<ContextInitializer>();
            services.AddDbContext<MemoryCellContext>(opts => opts.UseSqlite("Data Source=db/memorycell.db"));
            services.AddDbContext<BooruContext>(opts => opts.UseSqlite("Data Source=db/booru.db"));
            services.AddDbContext<PreferencesContext>(opts => opts.UseSqlite("Data Source=db/userprefs.db"));
            services.AddDbContext<KeywordContext>(opts => opts.UseSqlite("Data Source=db/keywords.db"));
            services.AddDbContext<ReplacementContext>(opts => opts.UseSqlite("Data Source=db/replacements.db"));
            services.AddDbContext<SuspicionContext>(opts => opts.UseSqlite("Data Source=db/suspicions.db"));
            services.AddDbContext<IdentityDbContext>(opts => opts.UseSqlite("Data Source=db/identity.db"));

            services.AddMemoryCache();
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<IdentityDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(opts =>
            {
                opts.User.AllowedUserNameCharacters = @"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+[]|1^{}\";
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = LogonService.Scheme;
                options.DefaultChallengeScheme = LogonService.Scheme;
                options.DefaultScheme = LogonService.Scheme;
            }).AddCookie(LogonService.Scheme, options =>
            {
                options.Events = new CookieAuthenticationEvents
                {
                    OnRedirectToLogin = context =>
                    {
                        if (context.Request.Path.StartsWithSegments("/api"))
                        {
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        }
                        else
                        {
                            context.Response.Redirect(context.RedirectUri);
                        }

                        return Task.FromResult(0);
                    }
                };
            });
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.All;
            });

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

            services.AddWebOptimizer(pipeline =>
            {
                pipeline.CompileScssFiles();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ChatBeet");
            });

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

            app.UseWebOptimizer();
            app.UseStaticFiles();
        }
    }
}
