using System;
using System.Collections.Generic;
using System.Text;

namespace SharedKernel.Exceptions
{
    public class CustomValidationException : Exception
    {
        public CustomValidationException() { }

        public CustomValidationException(string message)
            : base(message) { }

        public CustomValidationException(string message, Exception inner)
            : base(message, inner) { }
    }
}
