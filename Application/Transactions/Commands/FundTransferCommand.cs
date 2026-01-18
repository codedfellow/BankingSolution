using Application.DTOs.Account;
using Application.DTOs.Transactions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Transactions.Commands
{
    public record FundTransferCommand(string? DebitAccountNumber, string? CreditAccountNumber, decimal Amount) : IRequest<FundTransferResponse>;
}
