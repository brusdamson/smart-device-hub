using System.Threading.Tasks;
using UserService.Application.DTOs.Account.Requests;
using UserService.Application.DTOs.Account.Responses;
using UserService.Application.Wrappers;

namespace UserService.Application.Interfaces.UserInterfaces
{
    public interface IGetUserServices
    {
        Task<PagedResponse<UserDto>> GetPagedUsers(GetAllUsersRequest model);
    }
}
