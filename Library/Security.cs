using System;
using System.Security.Cryptography;
using System.Text;

namespace FiyiPOS.Library
{
    public static class Security
    {
        public static string EncryptString(string stringToEncrypt)
        {
            try
            {
                if (stringToEncrypt == string.Empty)
                {
                    return string.Empty;   
                }

                UnicodeEncoding UnicodeEncoding = new();

                string EncryptedString = Convert
                    .ToBase64String(SHA512
                    .HashData(UnicodeEncoding.GetBytes(stringToEncrypt)));

                return EncryptedString;
            }
            catch (Exception) { throw; }
        }
    }
}
