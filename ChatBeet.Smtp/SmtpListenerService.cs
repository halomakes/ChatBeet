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

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var options = new SmtpServerOptionsBuilder()
                .ServerName("localhost")
                .Port(25, 587)
                .MessageStore(new RadishMessageStore())
                .Build();

            server = new SmtpServer.SmtpServer(options);
            await server.StartAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
