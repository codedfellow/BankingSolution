using Application.Account;
using Application.Auth.Commands;
using Application.DTOs.Account;
using Application.DTOs.Auth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AccountController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Route("balance")]
        public async Task<IActionResult> AccountBalance([FromQuery]string? accountNumber)
        {
            return Ok("This works");
        }

        [HttpPost]
        [Route("open-account")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(OpenAccountResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> OpenAccount([FromBody]OpenAccountDto model)
        {
            var command = new OpenAccountCommand(model.UserId, model.Address);
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
