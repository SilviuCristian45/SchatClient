using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Security.Cryptography;

namespace schat
{
    class Utility
    {
        public static int userid;
        public static string currentusername;

        public static SqlConnection con;

        public static int MAXBUFFERSIZE = 255;
        public static string GenerateSalt()
        {
            byte[] bytes = new byte[128 / 8]; //maxim 16 bytes
            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(bytes); //umplem array-ul de bytes cu valori random secure 
            return Convert.ToBase64String(bytes); //returnam string-ul rezultat
        }

        public static string ComputeHash(string input, string psalt)
        {
            byte[] bytesToHash = Encoding.UTF8.GetBytes(input.ToCharArray(), 0, input.Length);
            byte[] salt = Encoding.UTF8.GetBytes(psalt.ToCharArray(), 0, psalt.Length);
            var byteResult = new Rfc2898DeriveBytes(bytesToHash, salt, 10000);
            return Convert.ToBase64String(byteResult.GetBytes(24));
        }
    }
}
