using com.caucho.model;
using Hessian.IO.Converters;
using Hessian.IO.Test.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Xunit;

namespace Hessian.IO.Test.Converters
{
    public class ConverterWriteTest : ConverterTestBase
    {
        [Fact]
        public void WriteBinary()
        {
            Serializer.Serialize(Stream, new byte[0]);
            Assert.Matches("x20", GetAndReset());

            Serializer.Serialize(Stream, new byte[15]);
            Assert.Matches("x2f(x00){15}", GetAndReset());

            Serializer.Serialize(Stream, new byte[1024 * 4 + 15]);
            Assert.Matches("x62x10x00(x00){4096}x2f(x00){15}", GetAndReset());

            Serializer.Serialize(Stream, new byte[1024 * 4 + 16]);
            Assert.Matches("x62x10x00(x00){4096}x42x00x10(x00){16}", GetAndReset());
        }

        [Fact]
        public void WriteBool()
        {
            Serializer.Serialize(Stream, true);
            ResetAndAssert("x54");

            Serializer.Serialize(Stream, false);
            ResetAndAssert("x46");
        }

        [Fact]
        public void WriteDateTime()
        {
            Serializer.Serialize(Stream, new DateTime(1998, 5, 8, 9, 51, 31, DateTimeKind.Utc));
            ResetAndAssert("x4ax00x00x00xd0x4bx92x84xb8");

            Serializer.Serialize(Stream, new DateTime(1998, 5, 8, 9, 51, 0, DateTimeKind.Utc));
            ResetAndAssert("x4bx00xe3x83x8f");
        }

        [Fact]
        public void WriteDouble()
        {
            Serializer.Serialize(Stream, 0d);
            ResetAndAssert("x5b");

            Serializer.Serialize(Stream, 1d);
            ResetAndAssert("x5c");

            Serializer.Serialize(Stream, -128d);
            ResetAndAssert("x5dx80");

            Serializer.Serialize(Stream, 127d);
            ResetAndAssert("x5dx7f");

            Serializer.Serialize(Stream, -32768d);
            ResetAndAssert("x5ex80x00");

            Serializer.Serialize(Stream, 32767d);
            ResetAndAssert("x5ex7fxff");

            Serializer.Serialize(Stream, 12.5d);
            ResetAndAssert("x5fx00x00x30xd4");

            Serializer.Serialize(Stream, 12.123d);
            ResetAndAssert("x44x40x28x3exf9xdbx22xd0xe5");
        }

        [Fact]
        public void WriteInt()
        {
            Serializer.Serialize(Stream, -16);
            ResetAndAssert("x80");

            Serializer.Serialize(Stream, 1);
            ResetAndAssert("x91");
            Serializer.Serialize(Stream, 16);
            ResetAndAssert("xa0");
            Serializer.Serialize(Stream, 256);
            ResetAndAssert("xc9x00");

            Serializer.Serialize(Stream, 47);
            ResetAndAssert("xbf");

            Serializer.Serialize(Stream, -2048);
            ResetAndAssert("xc0x00");

            Serializer.Serialize(Stream, 2047);
            ResetAndAssert("xcfxff");

            Serializer.Serialize(Stream, -262144);
            ResetAndAssert("xd0x00x00");

            Serializer.Serialize(Stream, 262143);
            ResetAndAssert("xd7xffxff");

            Serializer.Serialize(Stream, int.MinValue);
            ResetAndAssert("x49x80x00x00x00");

            Serializer.Serialize(Stream, int.MaxValue);
            ResetAndAssert("x49x7fxffxffxff");
        }

        [Fact]
        public void WriteLong()
        {
            Serializer.Serialize(Stream, -8L);
            ResetAndAssert("xd8");

            Serializer.Serialize(Stream, 15L);
            ResetAndAssert("xef");

            Serializer.Serialize(Stream, -2048L);
            ResetAndAssert("xf0x00");

            Serializer.Serialize(Stream, 2047L);
            ResetAndAssert("xffxff");

            Serializer.Serialize(Stream, -262144L);
            ResetAndAssert("x38x00x00");

            Serializer.Serialize(Stream, 262143L);
            ResetAndAssert("x3fxffxff");

            Serializer.Serialize(Stream, (long)int.MinValue);
            ResetAndAssert("x59x80x00x00x00");

            Serializer.Serialize(Stream, (long)int.MaxValue);
            ResetAndAssert("x59x7fxffxffxff");

            Serializer.Serialize(Stream, long.MinValue);
            ResetAndAssert("x4cx80x00x00x00x00x00x00x00");

            Serializer.Serialize(Stream, long.MaxValue);
            ResetAndAssert("x4cx7fxffxffxffxffxffxffxff");
        }

        [Fact]
        public void WriteList()
        {
            var stringType = Serializer.Serialize(typeof(string)).ToHexString();
            var ab = Serializer.Serialize("ab").ToHexString();

            //variable-length list
            var list = new List<string> { "ab", "ab" };
            Serializer.Serialize(Stream, list);
            Assert.Matches($"55{stringType}({ab}){{2}}x5a", GetAndReset());

            //variable-length untyped list
            var untypedList = new List<object> { "ab", "ab" };
            Serializer.Serialize(Stream, untypedList);
            Assert.Matches($"x57({ab}){{2}}x5a", GetAndReset());
        }

        [Fact]
        public void WriteArray()
        {
            var stringType = Serializer.Serialize(typeof(string)).ToHexString();
            var ab = Serializer.Serialize("ab").ToHexString();

            //fixed-length list
            var fixedList = new string[] { "ab", "ab", "ab", "ab", "ab", "ab", "ab", "ab" };
            Serializer.Serialize(Stream, fixedList);
            Assert.Matches($"x56{stringType}x98({ab}){{8}}", GetAndReset());

            //fixed-length short list
            var fixedShortList = new string[] { "ab", "ab" };
            Serializer.Serialize(Stream, fixedShortList);
            Assert.Matches($"x72{stringType}({ab}){{2}}", GetAndReset());

            //fixed-length untyped list
            var fixedUntypedList = new object[] { "ab", "ab", "ab", "ab", "ab", "ab", "ab", "ab" };
            Serializer.Serialize(Stream, fixedUntypedList);
            Assert.Matches($"x58x98({ab}){{8}}", GetAndReset());

            //fixed-length short list
            var fixedUntypedShortList = new object[] { "ab", "ab" };
            Serializer.Serialize(Stream, fixedUntypedShortList);
            Assert.Matches($"x7a({ab}){{2}}", GetAndReset());
        }

        [Fact]
        public void WriteMap()
        {
            var dict = new Dictionary<int, string> { { 1, "fee" }, { 16, "fie" }, { 256, "foe" } };
            Serializer.Serialize(Stream, dict);
            ResetAndAssert($"{H('H')}{H(1)}{H("fee")}{H(16)}{H("fie")}{H(256)}{H("foe")}{H('Z')}");
        }

        [Fact]
        public void WriteObject()
        {
            Serializer.Serialize(Stream, new Car("red", "corvette"));
            ResetAndAssert($"{H('C')}{H(typeof(Car).FullName)}{H(2)}{H("color")}{H("model")}x60{H("red")}{H("corvette")}");
        }

        [Fact]
        public void WriteEnum()
        {
            var type = Serializer.Serialize(typeof(DayOfWeek)).ToHexString();
            var name = Serializer.Serialize("name").ToHexString();
            var monday = Serializer.Serialize("Monday").ToHexString();

            Serializer.Serialize(Stream, DayOfWeek.Monday);
            ResetAndAssert($"x43{type}x91{name}x60{monday}");
        }

        [Fact]
        public void WriteString()
        {
            Serializer.Serialize(Stream, string.Empty);
            Assert.Matches("x00", GetAndReset());

            Serializer.Serialize(Stream, new string('a', 31));
            Assert.Matches("x1f(x61){31}", GetAndReset());

            Serializer.Serialize(Stream, new string('a', 1023));
            Assert.Matches("x33xff(x61){1023}", GetAndReset());

            Serializer.Serialize(Stream, new string('a', 1024 * 4));
            Assert.Matches("x53x10x00(x61){4096}", GetAndReset());

            Serializer.Serialize(Stream, new string('a', 1024 * 4 + 31));
            Assert.Matches("x52x10x00(x61){4096}x1f(x61){31}", GetAndReset());
        }

        [Fact]
        public void WriteType()
        {
            Serializer.AutoReset = false;
            var fileInfoType = Serializer.Serialize("System.IO.FileInfo").ToHexString();
            var stringType = Serializer.Serialize("System.String").ToHexString();

            Serializer.Serialize(Stream, typeof(System.IO.FileInfo));
            ResetAndAssert(fileInfoType);

            Serializer.Serialize(Stream, typeof(string));
            ResetAndAssert(stringType);

            Serializer.Serialize(Stream, typeof(System.IO.FileInfo));
            ResetAndAssert("x90");

            Serializer.Serialize(Stream, typeof(string));
            ResetAndAssert("x91");
        }

        [Fact]
        public void TypeRef()
        {
            Serializer.AutoReset = false;
            Serializer.Serialize(Stream, typeof(string));
            ResetAndAssert($"{H(typeof(string).FullName)}");

            Serializer.Serialize(Stream, typeof(string));
            ResetAndAssert($"{H(0)}");
        }

        [Fact]
        public void ClassRef()
        {
            Serializer.AutoReset = false;
            Serializer.Serialize(Stream, new Car("red", "corvette"));
            ResetAndAssert($"{H('C')}{H(typeof(Car).FullName)}{H(2)}{H("color")}{H("model")}x60{H("red")}{H("corvette")}");

            Serializer.Serialize(Stream, new Person { Name = "yns", Age = 22 });
            ResetAndAssert($"{H('C')}{H(typeof(Person).FullName)}{H(2)}{H("Name")}{H("Age")}x61{H("yns")}{H(22)}");
        }

        [Fact]
        public void ValueRef()
        {
            Serializer.AutoReset = false;

            //object
            var car = new Car("red", "corvette");
            Serializer.Serialize(Stream, car);
            GetAndReset();
            Serializer.Serialize(Stream, car);
            ResetAndAssert($"x51{H(0)}");

            //enum
            Serializer.Serialize(Stream, DayOfWeek.Monday);
            GetAndReset();
            Serializer.Serialize(Stream, DayOfWeek.Monday);
            ResetAndAssert($"x51{H(1)}");

            //map
            var dict = new Dictionary<int, string> { { 1, "fee" } };
            Serializer.Serialize(Stream, dict);
            GetAndReset();
            Serializer.Serialize(Stream, dict);
            ResetAndAssert($"x51{H(2)}");

            //array
            var array = new string[] { "ab", "ab" };
            Serializer.Serialize(Stream, array);
            GetAndReset();
            Serializer.Serialize(Stream, array);
            ResetAndAssert($"x51{H(3)}");

            //list
            var list = new List<string> { "ab", "ab" };
            Serializer.Serialize(Stream, list);
            GetAndReset();
            Serializer.Serialize(Stream, list);
            ResetAndAssert($"x51{H(4)}");
        }
    }
}
