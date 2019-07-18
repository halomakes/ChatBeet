using ChatBeet.Queuing;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SmtpServer;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ChatBeet.Smtp
{
    public class SmtpListenerService : IHostedService
    {
        private SmtpServer.SmtpServer server;
        private readonly IMessageQueueService queueService;
        private readonly SmtpListenerConfiguration config;

        public SmtpListenerService(IMessageQueueService queueService, IOptions<SmtpListenerConfiguration> options)
        {
            this.queueService = queueService;
            config = options.Value;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var builder = new SmtpServerOptionsBuilder()
                .ServerName(config.ServerName)
                .Port(config.Ports.ToArray())
                .MessageStore(new RadishMessageStore(queueService));
            if (config.UseAuth)
                builder = builder.UserAuthenticator(new BasicUserAuthenticator(config.AuthConfig));
            var options = builder.Build();

            server = new SmtpServer.SmtpServer(options);
            await server.StartAsync(cancellationToken);
            return;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
