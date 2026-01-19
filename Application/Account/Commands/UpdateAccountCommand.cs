using Application.DTOs.Account;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Account.Commands
{
    public sealed record UpdateAccountCommand(string? AccountNumber, string? Address, Guid Userid) : IRequest<UpdateAccountResponse>;
}
