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

        public static string ToHexString(this byte[] buffer)
        {
            var builder = new StringBuilder(buffer.Length * 3);
            for (int i = 0; i < buffer.Length; i++)
            {
                builder.Append(string.Format("x{0:x2}", buffer[i]));
            }
            return builder.ToString();
        }

        public static Stream ToStream(this string hexString)
        {
            var stream = new MemoryStream(hexString.Length / 3);
            for (int i = 0; i < stream.Capacity; i++)
            {
                var b = byte.Parse(hexString.Substring(i * 3 + 1, 2), System.Globalization.NumberStyles.HexNumber);
                stream.WriteByte(b);
            }
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }
    }
}
