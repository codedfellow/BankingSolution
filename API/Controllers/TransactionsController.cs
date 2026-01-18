using Application.DTOs;
using Application.DTOs.Account;
using Application.DTOs.Transactions;
using Application.Transactions.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TransactionsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly SessionInfo _sessionInfo;
        public TransactionsController(IMediator mediator, SessionInfo sessionInfo)
        {
            _mediator = mediator;
            _sessionInfo = sessionInfo;
        }

        [HttpPost]
        [Route("fund-transfer")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(FundTransferResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> FundTransfer([FromBody] FundTransferDto model)
        {
            var command = new FundTransferCommand(model.DebitAccountNumber, model.CreditAccountNumber, model.Amount, _sessionInfo.UserId, model.Narration);
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
