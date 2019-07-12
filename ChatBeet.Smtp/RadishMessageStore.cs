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
        public override async Task<SmtpResponse> SaveAsync(ISessionContext context, IMessageTransaction transaction, CancellationToken cancellationToken)
        {
            var textMessage = (ITextMessage)transaction.Message;
            var text = string.Empty;

            using (var reader = new StreamReader(textMessage.Content, Encoding.UTF8))
            {
                text = await reader.ReadToEndAsync();
            }
            Console.WriteLine(text);

            return SmtpResponse.Ok;
        }
    }
}
