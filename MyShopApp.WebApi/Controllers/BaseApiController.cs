using Microsoft.AspNetCore.Mvc;

namespace MyShopApp.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseApiController : ControllerBase
    {
    }
}
