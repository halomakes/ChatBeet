using ChatBeet.Queuing;
using SmtpServer;
using SmtpServer.Mail;
using SmtpServer.Protocol;
using SmtpServer.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatBeet.Smtp
{
    internal class RadishMessageStore : MessageStore
    {
        private readonly IMessageQueueService queueService;

        public RadishMessageStore(IMessageQueueService queueService)
        {
            this.queueService = queueService;
        }

        public override async Task<SmtpResponse> SaveAsync(ISessionContext context, IMessageTransaction transaction, CancellationToken cancellationToken)
        {
            var textMessage = (ITextMessage)transaction.Message;
            var message = await MimeKit.MimeMessage.LoadAsync(textMessage.Content);
            queueService.Push(QueuedEmailMessage.FromMimeMessage(message));

            return SmtpResponse.Ok;
        }
    }
}
