using ChatBeet.Queuing;
using Microsoft.Extensions.Hosting;
using SmtpServer;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatBeet.Smtp
{
    public class SmtpListenerService : IHostedService
    {
        private SmtpServer.SmtpServer server;
        private readonly IMessageQueueService queueService;

        public SmtpListenerService(IMessageQueueService queueService)
        {
            this.queueService = queueService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var options = new SmtpServerOptionsBuilder()
                .ServerName("localhost")
                .Port(25, 587)
                .MessageStore(new RadishMessageStore(queueService))
                .Build();

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
