using MeetSport.Options;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace AiCompany.Services.PasswordHasher
{
    public sealed class PasswordHasher : IPasswordHasher
    {
        private HashingOptions HashingOptions { get; }

        public PasswordHasher(IOptions<HashingOptions> hashingOptions)
        {
            HashingOptions = hashingOptions.Value;
        }

        public string Hash(string password)
        {
            string hashedPassword;
            byte[] salt = Convert.FromBase64String(HashingOptions.Salt);
            using (Rfc2898DeriveBytes algorithm = new Rfc2898DeriveBytes(
              password,
              salt,
              HashingOptions.Iterations,
              HashAlgorithmName.SHA512))
            {
                hashedPassword = Convert.ToBase64String(algorithm.GetBytes(HashingOptions.KeySize));
            }

            return hashedPassword;
        }

        public bool Check(string password, string hash)
        {
            string hashedPassword = Hash(password);

            return hash == hashedPassword;
        }
    }
}
