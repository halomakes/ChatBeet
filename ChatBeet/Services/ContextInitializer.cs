using ChatBeet.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ChatBeet.Services;

public class ContextInitializer : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private static readonly IEnumerable<Type> contextTypes = new List<Type> {
        typeof(MemoryCellContext),
        typeof(BooruContext),
        typeof(PreferencesContext),
        typeof(KeywordContext),
        typeof(ReplacementContext),
        typeof(SuspicionContext),
        typeof(ProgressContext),
        typeof(IrcLinkContext)
    };

    public ContextInitializer(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        foreach (var ctxType in contextTypes)
        {
            var ctx = scope.ServiceProvider.GetRequiredService(ctxType) as DbContext;
            await ctx.Database.EnsureCreatedAsync();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}