using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Rystem.Test.Accounting.Policies
{
    public class CheckSomething : IAuthorizationRequirement
    {

    }
    public class CheckOtherSomething : IAuthorizationRequirement
    {

    }
    public class MultiHandler : IAuthorizationHandler
    {
        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            var pendingRequirements = context.PendingRequirements.ToList();
            foreach (var pending in pendingRequirements)
            {
                context.Succeed(pending);
            }
            return Task.CompletedTask;
        }
    }

    public class CheckSomethingHandler : AuthorizationHandler<CheckSomething>
    {

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CheckSomething requirement)
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
