using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Utilities
{
    public class Util
    {
        public static string GenerateReference(int length)
        {
            const string pool = "ABCDEFGHJKLMNPQRSTUVWXYZ0123456789";
            var builder = new StringBuilder();

            var random = new Random();

            for (var i = 0; i < length; i++)
            {
                var c = pool[random.Next(0, pool.Length)];
                builder.Append(c);
            }

            return builder.ToString();
        }
    }
}
