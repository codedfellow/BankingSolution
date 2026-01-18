using Application.Contracts.Data;
using Application.DTOs.Account;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Account.Commands
{
    internal class UpdateAccountCommandHandler : IRequestHandler<UpdateAccountCommand, UpdateAccountResponse>
    {
        private readonly IAppDbContext _context;
        public UpdateAccountCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<UpdateAccountResponse> Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
        {
            await _context.BeginTransactionAsync(cancellationToken);

            try
            {
                var account = await _context.Accounts.FirstOrDefaultAsync(x => x.AccountNumber == request.AccountNumber);

                if (account is null)
                {
                    throw new CustomValidationException("Account not found");
                }

                if (account.UserId != request.Userid)
                {
                    throw new CustomValidationException("Account does not belong to logged in user");
                }

                account.Address = request.Address ?? string.Empty;
                account.AccountBalance = request.AccountBalance;
                account.UpdatedAtUtc = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                await _context.CommitTransactionAsync(cancellationToken);

                return new UpdateAccountResponse("Account updated successfully");
            }
            catch (Exception)
            {
                await _context.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
    }
}
