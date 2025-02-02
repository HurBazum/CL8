using System.Security.Cryptography;
using System.Text;

namespace CL8.UI.Infrastructure.Others
{
    public static class PasswordProtector
    {
        public static string Protect(string password)
        {
            var passwordToBytes = Encoding.UTF8.GetBytes(password);
            return string.Join("", SHA512.HashData(passwordToBytes));
        }
    }
}