using System;
using Microsoft.AspNetCore.Authorization;

namespace ZAK.Auth;

public class PassHandler : AuthorizationHandler<PassRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PassRequirement requirement)
    {
        context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
