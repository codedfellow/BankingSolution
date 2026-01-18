using Application.Auth.Commands;
using Application.Contracts.Data;
using Application.DTOs.Auth;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using Domain.Entitites;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using Application.Account;
using Application.DTOs.Account;
using SharedKernel.Exceptions;

namespace Application.Account.AccountOperations
{
    internal class OpenAccountCommandHandler : IRequestHandler<OpenAccountCommand, OpenAccountResponse>
    {
        private readonly IAppDbContext _context;
        public OpenAccountCommandHandler(IAppDbContext context)
        {
            _context = context;
        }
        public async Task<OpenAccountResponse> Handle(OpenAccountCommand request, CancellationToken cancellationToken)
        {
            if (request.UserId is null)
            {
                throw new CustomValidationException("Kindly specify the user Id");
            }

            var userExists = await _context.Users.AnyAsync(x => x.Id == request.UserId);

            if (!userExists)
            {
                throw new CustomValidationException("Specified user not found");
            }

            var accountNumber = await GetAccountNumber();

            // And ensure that the instantiation uses the correct type from the namespace:
            var newAccount = new CustomerAccount
            {
                AccountNumber = accountNumber,
                AccountBalance = 0m,
                CreatedAtUtc = DateTime.UtcNow,
                UserId = (Guid)request.UserId,
                Address = request.Address ?? string.Empty,
            };

            await _context.Accounts.AddAsync(newAccount, cancellationToken);
            await _context.SaveChangesAsync();

            return new OpenAccountResponse("Account created successfully", newAccount.AccountNumber);
        }

        async Task<string> GetAccountNumber()
        {
            string accountNumber = HelperMethods.GenerateRandomNumericString(10);
            
            bool accountExists = true;

            do
            {
                accountExists = await ConfirmAccoutNumberExists(accountNumber);
            }
            while (accountExists);

            return accountNumber;
        }

        async Task<bool> ConfirmAccoutNumberExists(string accountNumber) 
        {
            bool result = await _context.Accounts.AnyAsync(x => x.AccountNumber  == accountNumber);
            return result;
        }
    }
}