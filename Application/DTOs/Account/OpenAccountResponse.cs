using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Account
{
    public sealed record OpenAccountResponse(string Message, string AccountNumber);
}
