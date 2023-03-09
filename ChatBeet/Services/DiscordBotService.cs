using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using ChatBeet.Commands;
using ChatBeet.Notifications;
using DSharpPlus.Interactivity.Extensions;
using MediatR;
using MessageTransformCommandModule = ChatBeet.Commands.MessageTransformCommandModule;

namespace ChatBeet.Services;

public class DiscordBotService : BackgroundService
{
    private readonly DiscordClient _client;
    private readonly ILogger<DiscordBotService> _logger;
    private readonly IServiceProvider _services;

    public DiscordBotService(DiscordClient client, ILogger<DiscordBotService> logger, IServiceProvider services)
    {
        _client = client;
        _logger = logger;
        _services = services;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // just run forever
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        _client.GuildCreated += PublishMessage;
        _client.GuildUpdated += PublishMessage;
        _client.GuildAvailable += PublishMessage;
        _client.UseInteractivity(new InteractivityConfiguration()
        {
            PollBehaviour = PollBehaviour.KeepEmojis,
            Timeout = TimeSpan.FromSeconds(30),
        });
        var commands = _client.UseSlashCommands(new SlashCommandsConfiguration
        {
            Services = _services
        });
        commands.SlashCommandErrored += async (_, x) => await LogError("Slash command failed", x.Exception);
        commands.AutocompleteErrored += async (_, x) => await LogError("Autocomplete failed", x.Exception);
        commands.ContextMenuErrored += async (_, x) => await LogError("Context menu failed", x.Exception);
        _client.ClientErrored += (_, x) =>
        {
            _logger.LogError(x.Exception, "Discord client error");
            return Task.CompletedTask;
        };
        _client.ModalSubmitted += PublishMessage;
        _client.MessageCreated += PublishMessage;
        _client.MessageReactionAdded += PublishMessage;

        commands.RegisterCommands<AnilistCommandModule>();
        commands.RegisterCommands<BadBotCommandModule>();
        commands.RegisterCommands<BeerCommandModule>();
        commands.RegisterCommands<BirthdayCommandModule>();
        commands.RegisterCommands<BonkCommandModule>();
        commands.RegisterCommands<BooruCommandModule>();
        commands.RegisterCommands<ComplimentCommandModule>();
        commands.RegisterCommands<DoggoCommandModule>();
        commands.RegisterCommands<GoogleCommandModule>();
        commands.RegisterCommands<DefinitionCommandModule>();
        commands.RegisterCommands<SystemInfoCommandModule>();
        commands.RegisterCommands<SuspicionCommandModule>();
        commands.RegisterCommands<MessageTransformCommandModule>();
        commands.RegisterCommands<ProgressCommandModule>();
        commands.RegisterCommands<SentimentCommandModule>();
        commands.RegisterCommands<IrcCommandModule>();
        commands.RegisterCommands<SauceCommandModule>();
        commands.RegisterCommands<PreferencesCommandModule>();
        commands.RegisterCommands<WhipCommandModule>();
        commands.RegisterCommands<GameLookupCommandModule>();
        commands.RegisterCommands<HighGroundCommandModule>();
        commands.RegisterCommands<JokeCommandModule>();
        commands.RegisterCommands<LastFmCommandModule>();
        commands.RegisterCommands<PreferenceLookupCommandModule>();
        commands.RegisterCommands<SpeedometerCommandModule>();
        commands.RegisterCommands<WolframCommandModule>();
        commands.RegisterCommands<EightBallCommandModule>();
        commands.RegisterCommands<MiataCommandModule>();
        commands.RegisterCommands<UrbanDictionaryCommandModule>();
        commands.RegisterCommands<YoutubeCommandModule>();
        commands.RegisterCommands<KarmaCommandModule>();
        commands.RegisterCommands<QuoteCommandModule>();
        commands.RegisterCommands<PollCommandModule>();
        commands.RegisterCommands<UserTagsCommandModule>();
        await _client.ConnectAsync();
        await base.StartAsync(cancellationToken);
    }

    private async Task LogError(string v, Exception exception)
    {
        _logger.LogError(exception, v);
        await _services.GetRequiredService<DiscordLogService>().LogError(v, exception);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await _client.DisconnectAsync();
        await base.StopAsync(cancellationToken);
    }

    public override void Dispose()
    {
        _client.Dispose();
        base.Dispose();
    }

    private async Task PublishMessage<TEvent>(DiscordClient sender, TEvent @event) where TEvent : DiscordEventArgs
    {
        await PublishMessage(new DiscordNotification<TEvent>(@event, sender));
    }
    
    private async Task PublishMessage<T>(T @event)
    {
        if (@event is null)
            return;
        await using var scope = _services.CreateAsyncScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
#pragma warning disable CS4014
        mediator.Publish(@event);
#pragma warning restore CS4014
    }
}