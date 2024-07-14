
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using UserService.Application.DTOs.Account.Requests;
using UserService.Application.DTOs.Account.Responses;
using UserService.Application.Interfaces.UserInterfaces;
using UserService.Application.Wrappers;

namespace UserService.WebApi.Controllers.v1
{
    [ApiVersion("1")]
    public class AccountController(IAccountServices accountServices) : BaseApiController
    {
        [HttpPost]
        public async Task<BaseResult<AuthenticationResponse>> Authenticate(AuthenticationRequest request)
            => await accountServices.Authenticate(request);

        [HttpPost]
        public async Task<BaseResult> RegisterAccount([FromBody] RegistrationRequest request)
            => await accountServices.RegisterAccount(request);
        
        [HttpPut, Authorize]
        public async Task<BaseResult> ChangeUserName(ChangeUserNameRequest model)
            => await accountServices.ChangeUserName(model);

        [HttpPut, Authorize]
        public async Task<BaseResult> ChangePassword(ChangePasswordRequest model)
            => await accountServices.ChangePassword(model);
    }
}
