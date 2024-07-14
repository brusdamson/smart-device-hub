using System.Threading.Tasks;
using UserService.Application.DTOs.Account.Requests;
using UserService.Application.DTOs.Account.Responses;
using UserService.Application.Wrappers;

namespace UserService.Application.Interfaces.UserInterfaces
{
    public interface IAccountServices
    {
        Task<BaseResult<string>> RegisterGostAccount();
        Task<BaseResult> RegisterAccount(RegistrationRequest request);
        Task<BaseResult> ChangePassword(ChangePasswordRequest model);
        Task<BaseResult> ChangeUserName(ChangeUserNameRequest model);
        Task<BaseResult<AuthenticationResponse>> Authenticate(AuthenticationRequest request);
        Task<BaseResult<AuthenticationResponse>> AuthenticateByUserName(string username);

    }
}
