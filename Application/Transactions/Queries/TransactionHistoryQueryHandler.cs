using Application.Contracts.Data;
using Application.DTOs.Transactions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Transactions.Queries
{
    public class TransactionHistoryQueryHandler : IRequestHandler<TransactionHistoryQuery, List<TransactionHistoryDto>>
    {
        private readonly IAppDbContext _context;
        public TransactionHistoryQueryHandler(IAppDbContext context)
        {
            _context = context;
        }
        public async Task<List<TransactionHistoryDto>> Handle(TransactionHistoryQuery request, CancellationToken cancellationToken)
        {
            var transactions = await (from t in _context.Transactions
                               join da in _context.Accounts on t.DebitAccountId equals da.Id
                               join ca in _context.Accounts on t.CreditAccountId equals ca.Id
                               join debitUser in _context.Users on da.UserId equals debitUser.Id
                               join creditUser in _context.Users on ca.UserId equals creditUser.Id
                               where debitUser.Id == request.UserId || creditUser.Id == request.UserId
                               select new TransactionHistoryDto
                               {
                                   DebitAccountNumber = da.AccountNumber,
                                   CreditAccountNumber = ca.AccountNumber,
                                   DebitAccountOwner = $"{debitUser.FirstName} {debitUser.MiddleName} {debitUser.LastName}",
                                   CreditAccountOwner = $"{creditUser.FirstName} {creditUser.MiddleName} {creditUser.LastName}",
                                   TransactionDateUtc = t.CreatedAtUtc,
                                   Amount = t.Amount,
                                   TransactionRef = t.TransactionRef,
                                   Narration = t.Narration ?? string.Empty,
                               }).ToListAsync();

            return transactions;
        }
    }
}
