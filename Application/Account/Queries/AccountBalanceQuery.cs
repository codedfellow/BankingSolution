using Application.DTOs.Account;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Account.Queries
{
    public sealed record AccountBalanceQuery(string AccountNumber, Guid UserId) : IRequest<AccountBalanceResponse>;
}
