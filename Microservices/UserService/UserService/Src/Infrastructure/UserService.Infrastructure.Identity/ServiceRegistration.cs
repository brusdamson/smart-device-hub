using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using AuthLibrary;
using UserService.Application.Interfaces.UserInterfaces;
using UserService.Application.Wrappers;
using UserService.Infrastructure.Identity.Contexts;
using UserService.Infrastructure.Identity.Models;
using UserService.Infrastructure.Identity.Services;
using UserService.Infrastructure.Identity.Settings;

namespace UserService.Infrastructure.Identity
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddIdentityInfrastructure(this IServiceCollection services, IConfiguration configuration, bool UseInMemoryDatabase)
        {
            if (UseInMemoryDatabase)
            {
                services.AddDbContext<IdentityContext>(options =>
                    options.UseInMemoryDatabase(nameof(IdentityContext)));
            }
            else
            {
                services.AddDbContext<IdentityContext>(options =>
                    options.UseSqlServer(configuration.GetConnectionString("IdentityConnection")));
            }

            services.AddTransient<IGetUserServices, GetUserServices>();
            services.AddTransient<IAccountServices, AccountServices>();

            var identitySettings = configuration.GetSection(nameof(IdentitySettings)).Get<IdentitySettings>();

            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;

                options.User.RequireUniqueEmail = false;
                options.SignIn.RequireConfirmedEmail = identitySettings.RequireUniqueEmail;

                options.Password.RequireDigit = identitySettings.PasswordRequireDigit;
                options.Password.RequiredLength = identitySettings.PasswordRequiredLength;
                options.Password.RequireNonAlphanumeric = identitySettings.PasswordRequireNonAlphanumic;
                options.Password.RequireUppercase = identitySettings.PasswordRequireUppercase;
                options.Password.RequireLowercase = identitySettings.PasswordRequireLowercase;
            }).AddEntityFrameworkStores<IdentityContext>().AddDefaultTokenProviders();

            var jwtSettings = configuration.GetSection(nameof(AuthExtensions.JWTSettings)).Get<AuthExtensions.JWTSettings>();
            services.AddSingleton(jwtSettings);
            
            
            services.AddJwtAuthentication(jwtSettings, new JwtBearerEvents()
            {
                OnChallenge = async context =>
                {
                    context.HandleResponse();
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsJsonAsync(new BaseResult(new Error(ErrorCode.AccessDenied, "You are not Authorized")));
                },
                OnForbidden = async context =>
                {
                    context.Response.StatusCode = 403;
                    await context.Response.WriteAsJsonAsync(new BaseResult(new Error(ErrorCode.AccessDenied, "You are not authorized to access this resource")));
                },
                OnTokenValidated = async context =>
                {
                    var claimsIdentity = context.Principal.Identity as ClaimsIdentity;
                    if (claimsIdentity.Claims?.Any() is not true)
                        context.Fail("This token has no claims.");

                    var securityStamp = claimsIdentity.FindFirst("AspNet.Identity.SecurityStamp");
                    if (securityStamp is null)
                        context.Fail("This token has no secuirty stamp");

                    var signInManager = context.HttpContext.RequestServices.GetRequiredService<SignInManager<ApplicationUser>>();
                    var validatedUser = await signInManager.ValidateSecurityStampAsync(context.Principal);
                    if (validatedUser is null)
                        context.Fail("Token secuirty stamp is not valid.");
                },

            });
            
            return services;
        }
    }
}
