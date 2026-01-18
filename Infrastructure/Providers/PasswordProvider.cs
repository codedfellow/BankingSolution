using Application.Contracts.Providers;
using Domain.Entitites;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using static BCrypt.Net.BCrypt;

namespace Infrastructure.Providers
{
    internal class PasswordProvider : IPasswordProvider
    {
        public bool CustomConfirmPassword(string password, string passwordHash)
        {
            return Verify(password, passwordHash);
        }

        public string CustomHashPassword(string password)
        {
            string passwordHash = HashPassword(password);
            return passwordHash;
        }
    }
}
