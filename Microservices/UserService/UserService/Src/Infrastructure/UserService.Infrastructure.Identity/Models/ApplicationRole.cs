using Microsoft.AspNetCore.Identity;
using System;

namespace UserService.Infrastructure.Identity.Models
{
    public class ApplicationRole(string name) : IdentityRole<Guid>(name)
    {
    }
}
