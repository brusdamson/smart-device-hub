using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using UserService.WebApi.Infrastracture.Filters;

namespace UserService.WebApi.Controllers
{
    [ApiController]
    [ApiResultFilter]
    [Route("api/v{version:apiVersion}/[controller]/[action]")]
    public abstract class BaseApiController : ControllerBase
    {
        private IMediator _mediator;
        protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();
    }
}
