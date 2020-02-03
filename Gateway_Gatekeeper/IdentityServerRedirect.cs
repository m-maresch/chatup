using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Rewrite;

namespace Gateway_Gatekeeper
{
    public class IdentityServerRedirect : IRule
    {
        public void ApplyRule(RewriteContext context)
        {
            if (context.HttpContext.Request.Path.Value.Contains("/.well-known/openid-configuration"))
            {
                context.Result = RuleResult.SkipRemainingRules;
            }
        }
    }
}
