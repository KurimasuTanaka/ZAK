using System;
using Microsoft.AspNetCore.Authorization;

namespace ZAK.Auth;

public class PassRequirement : IAuthorizationRequirement
{
    public PassRequirement(string password)
    {
        Password = password;
    }
    public string Password { get; }
}
