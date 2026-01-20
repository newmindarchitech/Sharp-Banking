using Konscious.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
namespace BankingVault.Controllers
{
    public class UserController : Controller
    {
        //private readonly DatabaseContext db;
        private const int _MemorySize = 65536;
        private const int _DegreeOfParallelism = 2;
        private const int _Iteration = 4;
        private const int _HashSize = 32;
        private const int _SaltSize = 16;
        public IActionResult Index()
        {
            return View();
        }

        private string HashPassword(string password, byte[] salt)
        {
            byte[] passwordByte = Encoding.UTF8.GetBytes(password);

            using var argon2 = new Argon2id(passwordByte)
            {
                Salt = salt,
                MemorySize = _MemorySize,
                DegreeOfParallelism = _DegreeOfParallelism,
                Iterations = _Iteration
            };
            byte[] hash = argon2.GetBytes(_HashSize);
            return Convert.ToHexString(hash);
        }
        private byte[] CreateSalt()
        {
            return RandomNumberGenerator.GetBytes(_SaltSize);
        }

        private bool VerifyPassword(string password, byte[] salt, string passwordhash)
        {

            string hash = HashPassword(password, salt);

            return hash == passwordhash;
        }
    }
}
