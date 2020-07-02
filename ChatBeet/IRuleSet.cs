using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace ChatBeet
{
    public interface IRuleSet
    {
        IEnumerable<IMessageRule> GetRules();

        IApplicationBuilder ConfigureDependencies(IApplicationBuilder application, IConfiguration configuration);
    }
}
