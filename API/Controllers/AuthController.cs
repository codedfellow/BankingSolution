using Application.Auth.Commands;
using Application.DTOs.Auth;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Route("register")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(RegisterUserResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> RegisterUserAsync([FromBody]RegisterUserDto model)
        {
            var command = new RegisterUserCommand(model.FirstName, model.MiddleName, model.LastName, model.Email, model.Password);
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
