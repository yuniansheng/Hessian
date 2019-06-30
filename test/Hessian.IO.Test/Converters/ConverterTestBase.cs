using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace Hessian.IO.Test.Converters
{
    public class ConverterTestBase
    {
        public HessianWriter Writer { get; set; }

        public ConverterTestBase()
        {
            var stream = new MemoryStream();
            Writer = new HessianWriter(stream);
        }

        public void ResetAndAssert(string expected)
        {
            Assert.Equal(expected, GetAndReset());
        }

        public string GetAndReset()
        {
            Writer.Flush();
            Writer.BaseStream.Seek(0, SeekOrigin.Begin);
            var content = Writer.BaseStream.ToHexString();
            Writer.BaseStream.SetLength(0);
            return content;
        }
    }
}
