using Application.Account;
using Application.Account.Commands;
using Application.Account.Queries;
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
            var query = new AccountBalanceQuery(accountNumber ?? string.Empty);
            var result = await _mediator.Send(query);
            return Ok(result);
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

        [HttpPut]
        [Route("update-account")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(UpdateAccountResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdatedAccount([FromBody]UpdateAccountDto model)
        {
            var command = new UpdateAccountCommand(model.AccountNumber, model.AccountBalance, model.Address);
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
