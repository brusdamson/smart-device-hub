using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using AuthLibrary;
using Microsoft.AspNetCore.Identity;
using UserService.Application.DTOs.Account.Requests;
using UserService.Application.DTOs.Account.Responses;
using UserService.Application.Helpers;
using UserService.Application.Interfaces;
using UserService.Application.Interfaces.UserInterfaces;
using UserService.Application.Wrappers;
using UserService.Infrastructure.Identity.Models;

namespace UserService.Infrastructure.Identity.Services;

public class AccountServices(
    UserManager<ApplicationUser> userManager,
    IAuthenticatedUserService authenticatedUser,
    SignInManager<ApplicationUser> signInManager,
    AuthExtensions.JWTSettings jwtSettings,
    ITranslator translator) : IAccountServices
{
    public async Task<BaseResult> ChangePassword(ChangePasswordRequest model)
    {
        var user = await userManager.FindByIdAsync(authenticatedUser.UserId);

        var token = await userManager.GeneratePasswordResetTokenAsync(user);

        var identityResult = await userManager.ResetPasswordAsync(user, token, model.Password);

        if (identityResult.Succeeded)
            return new BaseResult();

        return new BaseResult(identityResult.Errors.Select(p => new Error(ErrorCode.ErrorInIdentity, p.Description)));
    }

    public async Task<BaseResult> ChangeUserName(ChangeUserNameRequest model)
    {
        var user = await userManager.FindByIdAsync(authenticatedUser.UserId);

        user.UserName = model.UserName;

        var identityResult = await userManager.UpdateAsync(user);

        if (identityResult.Succeeded)
            return new BaseResult();

        return new BaseResult(identityResult.Errors.Select(p => new Error(ErrorCode.ErrorInIdentity, p.Description)));
    }

    public async Task<BaseResult<AuthenticationResponse>> Authenticate(AuthenticationRequest request)
    {
        var user = await userManager.FindByNameAsync(request.UserName);
        if (user == null)
            return new BaseResult<AuthenticationResponse>(new Error(ErrorCode.NotFound,
                translator.GetString(
                    TranslatorMessages.AccountMessages.Account_notfound_with_UserName(request.UserName)),
                nameof(request.UserName)));

        var signInResult = await signInManager.PasswordSignInAsync(user.UserName, request.Password, false, false);
        if (!signInResult.Succeeded)
            return new BaseResult<AuthenticationResponse>(new Error(ErrorCode.FieldDataInvalid,
                translator.GetString(TranslatorMessages.AccountMessages.Invalid_password()), nameof(request.Password)));

        var result = await GetAuthenticationResponse(user);

        return new BaseResult<AuthenticationResponse>(result);
    }

    public async Task<BaseResult<AuthenticationResponse>> AuthenticateByUserName(string username)
    {
        var user = await userManager.FindByNameAsync(username);
        if (user == null)
            return new BaseResult<AuthenticationResponse>(new Error(ErrorCode.NotFound,
                translator.GetString(TranslatorMessages.AccountMessages.Account_notfound_with_UserName(username)),
                nameof(username)));

        var result = await GetAuthenticationResponse(user);

        return new BaseResult<AuthenticationResponse>(result);
    }

    public async Task<BaseResult> RegisterAccount(RegistrationRequest request)
    {
        var user = new ApplicationUser
        {
            UserName = request.UserName,
            Email = request.Email
        };

        var identityResult = await userManager.CreateAsync(user, request.Password);

        return identityResult.Succeeded
            ? new BaseResult()
            : new BaseResult(identityResult.Errors.Select(p => new Error(ErrorCode.ErrorInIdentity, p.Description)));
    }

    public async Task<BaseResult<string>> RegisterGhostAccount()
    {
        var user = new ApplicationUser
        {
            UserName = GenerateRandomString(7)
        };

        var identityResult = await userManager.CreateAsync(user);

        if (identityResult.Succeeded)
            return new BaseResult<string>(user.UserName);

        return new BaseResult<string>(
            identityResult.Errors.Select(p => new Error(ErrorCode.ErrorInIdentity, p.Description)));

        string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }

    private async Task<AuthenticationResponse> GetAuthenticationResponse(ApplicationUser user)
    {
        var jwToken = await GenerateJwtToken(user);

        var rolesList = await userManager.GetRolesAsync(user);

        return new AuthenticationResponse
        {
            Id = user.Id.ToString(),
            JWToken = new JwtSecurityTokenHandler().WriteToken(jwToken),
            Email = user.Email,
            UserName = user.UserName,
            Roles = rolesList,
            IsVerified = user.EmailConfirmed
        };

        async Task<JwtSecurityToken> GenerateJwtToken(ApplicationUser appUser)
        {
            await userManager.UpdateSecurityStampAsync(appUser);
            return AuthExtensions.GenerateJwtToken(jwtSettings,
                (await signInManager.ClaimsFactory.CreateAsync(appUser)).Claims);
        }
    }
}