using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Security
{
    public class Cryptograph
    {
        public static string ComputeHash(HashAlgorithm algorithm, string value)
        {
            var bytes = Encoding.UTF8.GetBytes(value);
            var cryptoBytes = algorithm.ComputeHash(bytes);
            var result = BitConverter.ToString(cryptoBytes).Replace("-", "");
            return result;
        }

        public static string ComputeHash(string value)
        {
            return ComputeHash(MD5.Create(), value);
        }

        public static Guid ComputeGuidHash(HashAlgorithm algorithm, string value)
        {
            var hash = ComputeHash(algorithm, value);
            return new Guid(hash);
        }

        public static Guid ComputeGuidHash(string value)
        {
            var hash = ComputeHash(MD5.Create(), value);
            return new Guid(hash);
        }
    }
}
