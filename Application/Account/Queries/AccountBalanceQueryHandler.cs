using Application.Contracts.Data;
using Application.DTOs.Account;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Account.Queries
{
    internal class AccountBalanceQueryHandler : IRequestHandler<AccountBalanceQuery, AccountBalanceResponse>
    {
        private readonly IAppDbContext _context;
        public AccountBalanceQueryHandler(IAppDbContext context)
        {
            _context = context;
        }
        public async Task<AccountBalanceResponse> Handle(AccountBalanceQuery request, CancellationToken cancellationToken)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(x => x.AccountNumber == request.AccountNumber);

            if (account is null)
            {
                throw new CustomValidationException("Account not found");
            }

            var result = await (from acc in _context.Accounts
                         join user in _context.Users on acc.UserId equals user.Id
                         where acc.AccountNumber == request.AccountNumber
                         select new AccountBalanceResponse(acc.AccountNumber, acc.AccountBalance, acc.Address, $"{user.FirstName} {user.MiddleName} {user.LastName}")).FirstOrDefaultAsync();

            return result;
        }
    }
}
