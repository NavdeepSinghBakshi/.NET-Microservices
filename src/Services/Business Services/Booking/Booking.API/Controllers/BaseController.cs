using Microsoft.AspNetCore.Mvc;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Booking.API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class BaseController : Controller
    {
        private IMediator _mediator;
        protected IMediator Mediator => _mediator ?? (_mediator = HttpContext.RequestServices.GetService<IMediator>());
    }
}
