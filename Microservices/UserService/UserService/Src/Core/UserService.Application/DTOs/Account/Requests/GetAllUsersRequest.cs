using UserService.Application.Parameters;

namespace UserService.Application.DTOs.Account.Requests
{
    public class GetAllUsersRequest : PagenationRequestParameter
    {
        public string Name { get; set; }
    }
}
