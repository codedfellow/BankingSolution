using Application.DTOs.Account;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Account
{
    public sealed record OpenAccountCommand(Guid? UserId, string? Address) : IRequest<OpenAccountResponse>;
}
