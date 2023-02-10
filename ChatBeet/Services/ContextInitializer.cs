using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using ChatBeet.Data;
using Microsoft.EntityFrameworkCore;

namespace ChatBeet.Services;

public class ContextInitializer : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public ContextInitializer(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<CbDbContext>();
        await ctx.Database.MigrateAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}