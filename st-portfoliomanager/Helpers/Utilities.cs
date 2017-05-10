using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace SofttrendsAddon.Helpers
{
    public static class Utilities
    {
        public static IServiceProvider AppServiceProvider { get; set; }
        public static ILoggerFactory AppLoggerFactory { get; set; }
        private static Random random = new Random();

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static T GetGuid<T>() where T : class
        {
            if (typeof(T) == typeof(string))
            {
                return Guid.NewGuid().ToString() as T;
            }
            else
            {
                return Guid.NewGuid() as T;
            }
        }

        public static string GetPGConnectionString(string pDatabaseUrl)
        {
            if (!string.IsNullOrEmpty(pDatabaseUrl))
            {
                string conStrParts = pDatabaseUrl.Replace("//", "");
                string[] strConn = conStrParts.Split(new char[] { '/', ':', '@', '?' });
                strConn = strConn.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                return string.Format("Host={0};Port={1};Database={2};User ID={3};Password={4};sslmode=Require;Trust Server Certificate=true;Pooling=true;MinPoolSize=1;MaxPoolSize=20;", strConn[3], strConn[4], strConn[5], strConn[1], strConn[2]);
            }

            return string.Empty;
        }

        public static string GetEnvVarVal(string pKey)
        {
            return Environment.GetEnvironmentVariable(pKey);
        }

        public static string SHA1HashStringForUTF8String(string pStr)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(pStr);

            var sha1 = SHA1.Create();
            byte[] hashBytes = sha1.ComputeHash(bytes);

            return HexStringFromBytes(hashBytes);
        }

        public static string HexStringFromBytes(byte[] bytes)
        {
            var sb = new StringBuilder();
            foreach (byte b in bytes)
            {
                var hex = b.ToString("x2");
                sb.Append(hex);
            }
            return sb.ToString();
        }

        public static long ConvertToUnixTime(DateTime datetime)
        {
            DateTime sTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            return (long)(datetime - sTime).TotalSeconds;
        }

        public static string RemoveSpecialChars(string strText)
        {
            return Regex.Replace(strText, @"[^0-9a-zA-Z\._]", string.Empty);
        }

        public static string EncryptText(string stringToEncrypt)
        {
            string encryptedValue = string.Empty;
            try
            {
                using (var aesAlg = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes("PortfolioManager@Sft123!", new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                    aesAlg.Key = pdb.GetBytes(32);
                    aesAlg.IV = pdb.GetBytes(16);
                    byte[] src = Encoding.Unicode.GetBytes(stringToEncrypt);
                    using (ICryptoTransform encrypt = aesAlg.CreateEncryptor())
                    {
                        byte[] dest = encrypt.TransformFinalBlock(src, 0, src.Length);
                        encryptedValue = Convert.ToBase64String(dest);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return encryptedValue;
        }
        public static string DecryptText(string encryptedText)
        {
            string decryptedValue = string.Empty;
            try
            {
                using (var aesAlg = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes("PortfolioManager@Sft123!", new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                    aesAlg.Key = pdb.GetBytes(32);
                    aesAlg.IV = pdb.GetBytes(16);
                    var src = Convert.FromBase64String(encryptedText);
                    using (ICryptoTransform decrypt = aesAlg.CreateDecryptor())
                    {
                        byte[] dest = decrypt.TransformFinalBlock(src, 0, src.Length);
                        decryptedValue = Encoding.Unicode.GetString(dest);
                    }
                }
            }
            catch (CryptographicException e)
            {
                Console.WriteLine(e.Message);
            }
            return decryptedValue;
        }

    }
}
