using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace ChatBeet.DefaultRules
{
    public class DefaultRuleSet : IRuleSet
    {
        public IApplicationBuilder ConfigureDependencies(IApplicationBuilder application, IConfiguration configuration)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IMessageRule> GetRules()
        {
            throw new NotImplementedException();
        }
    }
}
