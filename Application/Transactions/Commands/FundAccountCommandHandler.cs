using Application.Contracts.Data;
using Application.DTOs.Transactions;
using Domain.Entitites;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using SharedKernel.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Transactions.Commands
{
    internal class FundAccountCommandHandler : IRequestHandler<FundAccountCommand, FundTransferResponse>
    {
        private readonly IAppDbContext _context;
        public FundAccountCommandHandler(IAppDbContext context)
        {
            _context = context;
        }
        public async Task<FundTransferResponse> Handle(FundAccountCommand request, CancellationToken cancellationToken)
        {
            // Basic existence checks before starting a transaction
            var account = await _context.Accounts.FirstOrDefaultAsync(x => x.AccountNumber == request.AccountNumber, cancellationToken);

            if (account is null)
            {
                throw new CustomValidationException("Specified account not found");
            }

            if (account.UserId != request.UserId)
            {
                throw new CustomValidationException("Account does not belong to logged in user");
            }

            await _context.BeginTransactionAsync(cancellationToken);

            try
            {
                account.AccountBalance += request.Amount;

                string transactionReference = await GetTransactionReference();

                var transactionObject = new Transaction
                {
                    DebitAccountId = account.Id,
                    CreditAccountId = account.Id,
                    TransactionRef = transactionReference,
                    Amount = request.Amount,
                    Narration = request.Narration,
                    CreatedAtUtc = DateTime.UtcNow,
                };

                await _context.Transactions.AddAsync(transactionObject);
                await _context.SaveChangesAsync();
                await _context.CommitTransactionAsync(cancellationToken);

                return new FundTransferResponse("Transfer successful", transactionObject.TransactionRef);
            }
            catch (Exception)
            {
                await _context.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }

        async Task<string> GetTransactionReference()
        {
            string referenceNumber = HelperMethods.GenerateRandomNumericString(15);

            bool referenceExists = true;

            do
            {
                referenceExists = await ConfirmReferenceExists(referenceNumber);
            }
            while (referenceExists);

            return referenceNumber;
        }

        async Task<bool> ConfirmReferenceExists(string referenceNumber)
        {
            bool result = await _context.Transactions.AnyAsync(x => x.TransactionRef == referenceNumber);
            return result;
        }
    }
}
