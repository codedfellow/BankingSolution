using Domain.Entitites;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Contracts.Providers
{
    public interface IJwtTokenProvider
    {
        string GenerateToken(User user);
    }
}
