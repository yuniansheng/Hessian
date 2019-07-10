using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace Hessian.IO.Test.Converters
{
    public class ConverterReadTest : ConverterTestBase
    {
        public ConverterReadTest(ITestOutputHelper output) : base(output)
        {

        }

        [Theory]
        [InlineData(0)]
        [InlineData(15)]
        [InlineData(1024 * 4 + 15)]
        [InlineData(1024 * 4 + 16)]
        public void ReadBinary(int length)
        {
            var buffer = (byte[])Serializer.DeSerialize(S(new byte[length]));
            AssertLengthAndItemValue(buffer, length, (byte)0);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ReadBool(bool value)
        {
            var result = (bool)Serializer.DeSerialize(S(value));
            Assert.Equal(value, result);
        }

        [Fact]
        public void ReadDateTime()
        {
            var value = new DateTime(1998, 5, 8, 9, 51, 31, DateTimeKind.Utc);
            var result = (DateTime)Serializer.DeSerialize(S(value));
            Assert.Equal(value, result);

            value = new DateTime(1998, 5, 8, 9, 51, 0, DateTimeKind.Utc);
            result = (DateTime)Serializer.DeSerialize(S(value));
            Assert.Equal(value, result);
        }

        private void AssertLengthAndItemValue<T>(ICollection<T> collection, int length, T value)
        {
            Assert.Equal(length, collection.Count);
            Assert.All(collection, item => Assert.Equal(value, item));
        }
    }
}
