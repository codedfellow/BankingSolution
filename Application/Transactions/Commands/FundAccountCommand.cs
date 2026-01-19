using Application.DTOs.Transactions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Transactions.Commands
{
    public record FundAccountCommand(string? AccountNumber, decimal Amount, Guid UserId, string? Narration) : IRequest<FundTransferResponse>;
}
