using System.Security.Cryptography;
using System.Text;

namespace BasicWebLogin.Utilities
{
    public static class GeneralUtilities
    {
        public static string ConvertStringtoSHA256(string text)
        {
            string hash = string.Empty;
            using(SHA256 sha256 = SHA256.Create())
            {
                // Get hash from text
                byte[] hashValue = sha256.ComputeHash(Encoding.UTF8.GetBytes(text));

                // Convert byte array to string
                foreach(byte b in hashValue)
                    hash += $"{b:X2}";
            }

            return hash;
        }

        public static string CreateToken()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}
