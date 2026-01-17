using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Auth.Commands
{
    internal class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, string>
    {
        public RegisterUserCommandHandler()
        {
            
        }

        public async Task<string> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            
            return string.Empty;
        }
    }
}
