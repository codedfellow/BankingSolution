using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Auth.Commands
{
    public record RegisterUserCommand (string? FirstName, string? MiddleName, string? LastName, string? Email, string? Password) : IRequest<string>;
}
