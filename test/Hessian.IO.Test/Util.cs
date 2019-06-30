using System;
using System.IO;
using System.Text;
using Xunit;

namespace Hessian.IO.Test
{
    public static class Util
    {
        public static string ToHexString(this Stream stream)
        {
            var builder = new StringBuilder((int)stream.Length * 3);
            int n;
            while ((n = stream.ReadByte()) != -1)
            {
                builder.Append(string.Format("x{0:x2}", n));
            }
            return builder.ToString();
        }
    }
}
