using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomAuthorizationRequirement
{
    public class IsContributor : AuthorizationHandler<AdministratorRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdministratorRequirement requirement)
        {
            if(!context.User.Claims.Any(x=>x.Type.ToLower().Contains("role")&&(x.Value.ToLower().Contains("admin")||x.Value.ToLower().Contains("contributor"))))
            {
                context.Fail();
            }
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
