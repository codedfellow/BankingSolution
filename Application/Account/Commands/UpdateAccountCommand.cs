using Application.DTOs.Account;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Account.Commands
{
    public sealed record UpdateAccountCommand(string? AccountNumber, decimal AccountBalance, string? Address) : IRequest<UpdateAccountResponse>;
}
