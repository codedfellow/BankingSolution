using Application.Contracts.Data;
using Application.Contracts.Providers;
using Application.DTOs.Auth;
using Domain.Entitites;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Application.Auth.Commands
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, RegisterUserResponse>
    {
        private readonly IAppDbContext _context;
        private readonly IJwtTokenProvider _jwtTokenProvider;
        private readonly IPasswordProvider _passwordProvider;
        public RegisterUserCommandHandler(IAppDbContext context, IJwtTokenProvider jwtTokenProvider, IPasswordProvider passwordProvider)
        {
            _context = context;
            _jwtTokenProvider = jwtTokenProvider;
            _passwordProvider = passwordProvider;
        }

        public async Task<RegisterUserResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.FirstName) || string.IsNullOrEmpty(request.LastName))
            {
                throw new CustomValidationException("First name and last name are required");
            }

            if (string.IsNullOrEmpty(request.Password))
            {
                throw new CustomValidationException("Password is required required");
            }

            string email = request.Email?.Trim() ?? string.Empty;
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            if (!emailRegex.IsMatch(email) || string.IsNullOrEmpty(email))
            {
                throw new CustomValidationException("Email is not a valid email address");
            }

            bool userExists = await _context.Users.AnyAsync(u => u.Email.Trim().ToLower() == request.Email.Trim().ToLower(), cancellationToken);

            if (userExists)
            {
                throw new CustomValidationException("User with this email already exists.");
            }

            var user = new User
            {
                FirstName = request.FirstName.Trim(),
                MiddleName = request.MiddleName,
                LastName = request.LastName.Trim(),
                Email = email.Trim(),
                PasswordHash = _passwordProvider.CustomHashPassword(request.Password),
                CreatedAtUtc = DateTime.UtcNow,
            };

            await _context.Users.AddAsync(user, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            string token = _jwtTokenProvider.GenerateToken(user);

            return new RegisterUserResponse("User registration successful", token);
        }
    }
}
