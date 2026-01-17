using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Contracts.Providers
{
    public interface IPasswordProvider
    {
        bool CustomConfirmPassword(string password, string passwordHash);
        string CustomHashPassword(string password);
    }
}
