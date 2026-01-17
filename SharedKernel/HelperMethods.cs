using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace SharedKernel
{
    public static class HelperMethods
    {
        public static string GenerateRandomNumericString(int length)
        {
            if (length <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length), "Length must be a positive integer.");
            }

            // Use cryptographically secure random number generator
            using var rng = RandomNumberGenerator.Create();
            var randomBytes = new byte[length];
            rng.GetBytes(randomBytes);

            // Convert random bytes to digits (0-9)
            var result = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                // Map byte value to a digit (0-9)
                result.Append((randomBytes[i] % 10).ToString());
            }

            return result.ToString();
        }
    }
}
