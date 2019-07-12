using com.caucho.model;
using Hessian.IO.Converters;
using java.lang;
using System;
using System.Collections.Generic;
using System.IO;
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

        [Fact]
        public void ReadArray()
        {
            //fixed-length list
            var fixedList = new string[] { "ab", "ab", "ab", "ab", "ab", "ab", "ab", "ab" };
            fixedList = (string[])Serializer.DeSerialize(S(fixedList));
            AssertLengthAndItemValue(fixedList, 8, "ab");

            //fixed-length short list
            var fixedShortList = new string[] { "ab", "ab" };
            fixedShortList = (string[])Serializer.DeSerialize(S(fixedShortList));
            AssertLengthAndItemValue(fixedShortList, 2, "ab");

            //fixed-length untyped list
            var fixedUntypedList = new object[] { "ab", "ab", "ab", "ab", "ab", "ab", "ab", "ab" };
            fixedUntypedList = (object[])Serializer.DeSerialize(S(fixedUntypedList));
            AssertLengthAndItemValue(fixedUntypedList, 8, "ab");

            //fixed-length short list
            var fixedUntypedShortList = new object[] { "ab", "ab" };
            fixedUntypedShortList = (object[])Serializer.DeSerialize(S(fixedUntypedShortList));
            AssertLengthAndItemValue(fixedUntypedShortList, 2, "ab");
        }

        [Fact]
        public void ReadList()
        {
            //variable-length list
            var list = new List<string> { "ab", "ab" };
            list = (List<string>)Serializer.DeSerialize(S(list));
            AssertLengthAndItemValue(list, 2, "ab");

            //variable-length untyped list
            var untypedList = new List<object> { "ab", "ab" };
            untypedList = (List<object>)Serializer.DeSerialize(S(untypedList));
            AssertLengthAndItemValue(untypedList, 2, "ab");
        }

        [Fact]
        public void ReadMap()
        {
            var dict = new Dictionary<int, string> { { 1, "fee" }, { 16, "fie" }, { 256, "foe" } };
            var result = (Dictionary<object, object>)Serializer.DeSerialize(S(dict));
            Assert.Equal(3, result.Count);
            Assert.Equal("fee", result[1]);
            Assert.Equal("fie", result[16]);
            Assert.Equal("foe", result[256]);
        }

        [Fact]
        public void ReadObject()
        {
            var value = new Car("red", "corvette");
            var result = (Car)Serializer.DeSerialize(S(value));
            Assert.Equal(value, result);
        }

        [Fact]
        public void ReadEnum()
        {
            var result = Serializer.DeSerialize(S(DayOfWeek.Friday));
            Assert.Equal(DayOfWeek.Friday, result);
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

        [Fact]
        public void ReadClassDefinition()
        {
            Serializer.AutoReset = false;
            var writer = new HessianWriter(Stream);
            var context = new HessianContext();
            HessianConverter.ClassDefinitionConverter.WriteClassDefinition(writer, context, ClassDefinition.ForType(typeof(Car)), out int index);

            Stream.Seek(0, System.IO.SeekOrigin.Begin);
            var reader = new HessianReader(Stream);
            context = new HessianContext();
            var definition = (ClassDefinition)HessianConverter.ClassDefinitionConverter.ReadValue(reader, context, typeof(ClassDefinition));
            Assert.Equal(typeof(Car), definition.Type);
            Assert.Equal(2, definition.FieldNames.Count);
            Assert.Equal(nameof(Car.color), definition.FieldNames[0]);
            Assert.Equal(nameof(Car.model), definition.FieldNames[1]);
        }

        [Fact]
        public void TypeRef()
        {
            Serializer.AutoReset = false;
            Serializer.Serialize(Stream, typeof(string));
            Serializer.Serialize(Stream, typeof(string));

            Stream.Seek(0, System.IO.SeekOrigin.Begin);
            var reader = new HessianReader(Stream);
            var context = new HessianContext();
            var type1 = (Type)HessianConverter.TypeConverter.ReadValue(reader, context, null);
            Assert.Equal(typeof(string), type1);

            var type2 = (Type)HessianConverter.TypeConverter.ReadValue(reader, context, null);
            Assert.Same(type1, type2);
        }

        [Fact]
        public void ValueRef()
        {
            Serializer.AutoReset = false;

            //object
            var car = new Car("red", "corvette");
            Serializer.Serialize(Stream, car);
            Serializer.Serialize(Stream, car);

            //enum
            Serializer.Serialize(Stream, DayOfWeek.Monday);
            Serializer.Serialize(Stream, DayOfWeek.Monday);

            //map
            var dict = new Dictionary<int, string> { { 1, "fee" } };
            Serializer.Serialize(Stream, dict);
            Serializer.Serialize(Stream, dict);

            //array
            var array = new string[] { "ab", "ab" };
            Serializer.Serialize(Stream, array);
            Serializer.Serialize(Stream, array);

            //list
            var list = new List<string> { "ab", "ab" };
            Serializer.Serialize(Stream, list);
            Serializer.Serialize(Stream, list);

            Stream.Seek(0, SeekOrigin.Begin);
            Serializer.Reset();

            var car1 = Serializer.DeSerialize(Stream);
            var car2 = Serializer.DeSerialize(Stream);
            Assert.Same(car1, car2);

            var enum1 = Serializer.DeSerialize(Stream);
            var enum2 = Serializer.DeSerialize(Stream);
            Assert.Same(enum1, enum2);

            var map1 = Serializer.DeSerialize(Stream);
            var map2 = Serializer.DeSerialize(Stream);
            Assert.Same(map1, map2);

            var arr1 = Serializer.DeSerialize(Stream);
            var arr2 = Serializer.DeSerialize(Stream);
            Assert.Same(arr1, arr2);

            var list1 = Serializer.DeSerialize(Stream);
            var list2 = Serializer.DeSerialize(Stream);
            Assert.Same(list1, list2);
        }

        [Fact]
        public void ReadXxlJob()
        {
            XxlJob job = new XxlJob();
            job.requestParameterTypes = new Class[2];
            job.requestParameterTypes[0] = Class.OfName("int");
            job.requestParameterTypes[1] = Class.OfType<Car>();
            job.requestParameters = new object[2];
            job.requestParameters[0] = 1;
            job.requestParameters[1] = new Car("red", "corvette");
            job.responseMessage = "ok";
            job.responseResult = new GenericType(200, "hello");
            Serializer.Serialize(Stream, job);

            Stream.Seek(0, SeekOrigin.Begin);
            Serializer.Reset();
            var result = (XxlJob)Serializer.DeSerialize(Stream);

            Assert.Equal(job.requestParameterTypes[0].name, result.requestParameterTypes[0].name);
            Assert.Equal(job.requestParameterTypes[1].name, result.requestParameterTypes[1].name);
            Assert.Equal(job.requestParameters[0], result.requestParameters[0]);
            Assert.Equal(job.requestParameters[1], result.requestParameters[1]);
            Assert.Equal(job.responseMessage, result.responseMessage);
            Assert.Equal(job.responseResult, result.responseResult);
        }

        [Fact]
        public void ReadXxlJobFromJava()
        {
            XxlJob job = new XxlJob();
            job.requestParameterTypes = new Class[2];
            job.requestParameterTypes[0] = Class.OfName("int");
            job.requestParameterTypes[1] = Class.OfType<Car>();
            job.requestParameters = new object[2];
            job.requestParameters[0] = 1;
            job.requestParameters[1] = new Car("red", "corvette");
            job.responseMessage = "ok";
            job.responseResult = new GenericType(200, "hello");            

            var hexString = "x43x17x63x6fx6dx2ex63x61x75x63x68x6fx2ex6dx6fx64x65x6cx2ex58x78x6cx4ax6fx62x94x0fx72x65x73x70x6fx6ex73x65x4dx65x73x73x61x67x65x15x72x65x71x75x65x73x74x50x61x72x61x6dx65x74x65x72x54x79x70x65x73x11x72x65x71x75x65x73x74x50x61x72x61x6dx65x74x65x72x73x0ex72x65x73x70x6fx6ex73x65x52x65x73x75x6cx74x60x02x6fx6bx72x10x5bx6ax61x76x61x2ex6cx61x6ex67x2ex43x6cx61x73x73x43x0fx6ax61x76x61x2ex6cx61x6ex67x2ex43x6cx61x73x73x91x04x6ex61x6dx65x61x03x69x6ex74x61x14x63x6fx6dx2ex63x61x75x63x68x6fx2ex6dx6fx64x65x6cx2ex43x61x72x72x07x5bx6fx62x6ax65x63x74x91x43x14x63x6fx6dx2ex63x61x75x63x68x6fx2ex6dx6fx64x65x6cx2ex43x61x72x92x05x63x6fx6cx6fx72x05x6dx6fx64x65x6cx62x03x72x65x64x08x63x6fx72x76x65x74x74x65x43x1cx63x6fx6dx2ex63x61x75x63x68x6fx2ex6dx6fx64x65x6cx2ex47x65x6ex65x72x69x63x54x79x70x65x92x04x63x6fx64x65x04x64x61x74x61x63xc8xc8x05x68x65x6cx6cx6f";
            var result = (XxlJob)Serializer.DeSerialize(hexString.ToStream());

            Assert.Equal(job.requestParameterTypes[0].name, result.requestParameterTypes[0].name);
            Assert.Equal(job.requestParameterTypes[1].name, result.requestParameterTypes[1].name);
            Assert.Equal(job.requestParameters[0], result.requestParameters[0]);
            Assert.Equal(job.requestParameters[1], result.requestParameters[1]);
            Assert.Equal(job.responseMessage, result.responseMessage);
            Assert.Equal(job.responseResult, result.responseResult);
        }

        private void AssertLengthAndItemValue<T>(ICollection<T> collection, int length, T value)
        {
            Assert.Equal(length, collection.Count);
            Assert.All(collection, item => Assert.Equal(value, item));
        }
    }
}
