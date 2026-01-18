using Application.DTOs.Auth;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Auth.Commands
{
    public sealed record class LoginCommand(string? Email, string? Password) : IRequest<LoginResponse>;
}
