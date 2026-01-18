using Application.Account.Commands;
using Application.Contracts.Data;
using Application.DTOs.Account;
using Application.DTOs.Transactions;
using Domain.Entitites;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using SharedKernel.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Transactions.Commands
{
    internal class FundTransferCommandHandler : IRequestHandler<FundTransferCommand, FundTransferResponse>
    {
        private readonly IAppDbContext _context;
        private readonly IMediator _mediator;
        public FundTransferCommandHandler(IAppDbContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }
        public async Task<FundTransferResponse> Handle(FundTransferCommand request, CancellationToken cancellationToken)
        {
            // Basic existence checks before starting a transaction
            var debitAccount = await _context.Accounts.FirstOrDefaultAsync(x => x.AccountNumber == request.DebitAccountNumber, cancellationToken);

            if (debitAccount is null)
            {
                throw new CustomValidationException("Specified debit account not found");
            }

            if (debitAccount.UserId != request.UserId)
            {
                throw new CustomValidationException("Debit account does not belong to logged in user");
            }

            var creditAccount = await _context.Accounts.FirstOrDefaultAsync(x => x.AccountNumber == request.CreditAccountNumber, cancellationToken);

            if (creditAccount is null)
            {
                throw new CustomValidationException("Specified credit account not found");
            }

            bool canBedebited = debitAccount.AccountBalance >= request.Amount;

            if (!canBedebited)
            {
                throw new CustomValidationException($"Insufficient funds. Kindly fund account with at least {request.Amount - debitAccount.AccountBalance}");
            }

            // Start a DB transaction and re-query the rows with update locks to avoid race conditions
            await _context.BeginTransactionAsync(cancellationToken);

            try
            {
                debitAccount.AccountBalance -= request.Amount;
                creditAccount.AccountBalance += request.Amount;

                string transactionReference = await GetTransactionReference();

                var transactionObject = new Transaction
                {
                    DebitAccountId = debitAccount.Id,
                    CreditAccountId = creditAccount.Id,
                    TransactionRef = transactionReference,
                    Amount = request.Amount,
                    Narration = request.Narration,
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
