using Hessian.IO.Converters;
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

        [Theory]
        [InlineData(0d)]
        [InlineData(1d)]
        [InlineData(-128d)]
        [InlineData(127d)]
        [InlineData(32768d)]
        [InlineData(32767d)]
        [InlineData(12.5d)]
        [InlineData(12.123d)]
        public void ReadDouble(double value)
        {
            var result = (double)Serializer.DeSerialize(S(value));
            Assert.Equal(value, result);
        }

        [Theory]
        [InlineData(-16)]
        [InlineData(1)]
        [InlineData(16)]
        [InlineData(256)]
        [InlineData(47)]
        [InlineData(-2048)]
        [InlineData(2047)]
        [InlineData(-262144)]
        [InlineData(262143)]
        [InlineData(int.MinValue)]
        [InlineData(int.MaxValue)]
        public void ReadInt(int value)
        {
            var result = (int)Serializer.DeSerialize(S(value));
            Assert.Equal(value, result);
        }

        [Theory]
        [InlineData(-8L)]
        [InlineData(15L)]
        [InlineData(-2048L)]
        [InlineData(2047L)]
        [InlineData(-262144L)]
        [InlineData(262143L)]
        [InlineData((long)int.MinValue)]
        [InlineData((long)int.MaxValue)]
        [InlineData(long.MinValue)]
        [InlineData(long.MaxValue)]
        public void ReadLong(long value)
        {
            var result = (long)Serializer.DeSerialize(S(value));
            Assert.Equal(value, result);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(31)]
        [InlineData(1023)]
        [InlineData(1024 * 4)]
        [InlineData(1024 * 4 + 31)]
        public void ReadString(int len)
        {
            var result = (string)Serializer.DeSerialize(S(new string('a', len)));
            Assert.Equal(len, result.Length);
        }

        [Fact]
        public void ReadType()
        {
            var stream = S(typeof(string));
            var result = HessianConverter.TypeConverter.ReadValue(new HessianReader(stream), new HessianContext(), null);
            Assert.Equal(typeof(string), result);

            stream = S(typeof(string[]));
            result = HessianConverter.TypeConverter.ReadValue(new HessianReader(stream), new HessianContext(), null);
            Assert.Equal(typeof(string[]), result);

            stream = S(typeof(List<string>));
            result = HessianConverter.TypeConverter.ReadValue(new HessianReader(stream), new HessianContext(), null);
            Assert.Equal(typeof(List<string>), result);
        }

        private void AssertLengthAndItemValue<T>(ICollection<T> collection, int length, T value)
        {
            Assert.Equal(length, collection.Count);
            Assert.All(collection, item => Assert.Equal(value, item));
        }
    }
}
