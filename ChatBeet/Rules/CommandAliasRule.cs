using GravyBot;
using GravyBot.Commands;
using GravyIrc.Messages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ChatBeet.Rules
{
    public abstract class CommandAliasRule<TProcessor> : IAsyncMessageRule<PrivateMessage> where TProcessor : CommandProcessor
    {
        private IServiceProvider ServiceProvider;
        protected IrcBotConfiguration Configuration;
        protected Regex Pattern;
        protected string SimulatedCommandName;

        public CommandAliasRule(IOptions<IrcBotConfiguration> options, IServiceProvider serviceProvider)
        {
            Configuration = options.Value;
            ServiceProvider = serviceProvider;
        }

        public virtual bool Matches(PrivateMessage incomingMessage) => Pattern.IsMatch(incomingMessage.Message);

        public virtual IAsyncEnumerable<IClientMessage> RespondAsync(PrivateMessage incomingMessage)
        {
            var processor = ServiceProvider.GetService<TProcessor>();
            processor.IncomingMessage = incomingMessage;
            processor.TriggeringCommandName = SimulatedCommandName;
            return OnMatch(Pattern.Match(incomingMessage.Message), processor);
        }

        protected abstract IAsyncEnumerable<IClientMessage> OnMatch(Match match, TProcessor commandProcessor);
    }
}
