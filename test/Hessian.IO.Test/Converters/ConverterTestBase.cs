using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace Hessian.IO.Test.Converters
{
    public class ConverterTestBase
    {
        public Stream Stream { get; set; }

        public HessianSerializer Serializer { get; set; }

        public ConverterTestBase()
        {
            Stream = new MemoryStream();
            Serializer = new HessianSerializer();
            Serializer.AutoReset = true;
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
            return content;
        }
    }
}
