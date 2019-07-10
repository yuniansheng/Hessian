using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace Hessian.IO.Test.Converters
{
    public class ConverterTestBase
    {
        public Stream Stream { get; set; }

        public HessianSerializer Serializer { get; set; }

        private readonly ITestOutputHelper output;

        public ConverterTestBase(ITestOutputHelper output)
        {
            Stream = new MemoryStream();
            Serializer = new HessianSerializer();
            Serializer.AutoReset = true;
            this.output = output;
        }

        public void ResetAndAssert(string expected)
        {
            Assert.Equal(expected, GetAndReset());
        }

        public string GetAndReset()
        {
            Stream.Seek(0, SeekOrigin.Begin);
            var content = Stream.ToHexString();
            Stream.SetLength(0);
            output.WriteLine(content);
            return content;
        }

        public string H(object value)
        {
            if (value is char)
            {
                return string.Format("x{0:x2}", (byte)(char)value);
            }
            return new HessianSerializer().Serialize(value).ToHexString();
        }

        public Stream S(object value)
        {
            var serializer = new HessianSerializer();
            var stream = new MemoryStream();
            serializer.Serialize(stream, value);
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }
    }
}
