using Application.Contracts.Data;
using Application.Contracts.Providers;
using Application.DTOs.Auth;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Auth.Commands
{
    internal class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
    {
        private readonly IAppDbContext _context;
        private readonly IJwtTokenProvider _jwtTokenProvider;
        private readonly IPasswordProvider _passwordProvider;
        public LoginCommandHandler(IAppDbContext context, IJwtTokenProvider jwtTokenProvider, IPasswordProvider passwordProvider)
        {
            _context = context;
            _jwtTokenProvider = jwtTokenProvider;
            _passwordProvider = passwordProvider;
        }
        public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                throw new CustomValidationException("Email and password are required");
            }

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email ==  request.Email);

            if (user == null || !_passwordProvider.CustomConfirmPassword(request.Password, user.PasswordHash))
            {
                throw new Exception("Invalid email or password.");
            }

            string token = _jwtTokenProvider.GenerateToken(user);

            return new LoginResponse("Login successful", token);
        }
    }
}