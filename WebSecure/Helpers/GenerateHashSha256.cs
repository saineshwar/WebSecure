using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WebSecure.Helpers
{
    public static class GenerateHashSha256
    {
        public static string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                foreach (var t in bytes)
                {
                    builder.Append(t.ToString("x2"));
                }

                return builder.ToString();
            }
        }
    }

    public static class GeneratePageHash
    {
        public static string Hash(string valuetohash)
        {
            if (!string.IsNullOrEmpty(valuetohash))
            {
                return GenerateHashSha256.ComputeSha256Hash(valuetohash);
            }
            else
            {
                throw new System.ArgumentException("Parameter cannot be null", $"GeneratePageHash:valuetohash");
            }

        }
    }
}
