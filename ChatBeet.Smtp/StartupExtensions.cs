using Microsoft.Extensions.DependencyInjection;

namespace ChatBeet.Smtp
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddSmtpListener(this IServiceCollection services)
        {
            services.AddHostedService<SmtpListenerService>();
            return services;
        }
    }
}
