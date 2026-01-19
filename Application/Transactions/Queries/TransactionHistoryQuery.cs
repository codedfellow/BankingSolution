using Application.DTOs.Transactions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Transactions.Queries
{
    public sealed record TransactionHistoryQuery(Guid UserId) : IRequest<List<TransactionHistoryDto>>;
}
