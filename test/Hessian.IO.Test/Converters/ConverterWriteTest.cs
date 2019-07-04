using com.caucho.model;
using Hessian.IO.Converters;
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
            string stringType = "x0dx53x79x73x74x65x6dx2ex53x74x72x69x6ex67";

            //variable-length list
            var list = new List<string> { "ab", "ab" };
            Serializer.Serialize(Stream, list);
            Assert.Matches($"55{stringType}(x02x61x62){{2}}x5a", GetAndReset());
            Serializer.Reset();

            //variable-length untyped list
            var untypedList = new List<object> { "ab", "ab" };
            Serializer.Serialize(Stream, untypedList);
            Assert.Matches($"x57(x02x61x62){{2}}x5a", GetAndReset());
        }

        [Fact]
        public void WriteArray()
        {
            string stringType = "x0dx53x79x73x74x65x6dx2ex53x74x72x69x6ex67";

            //fixed-length list
            var fixedList = new string[] { "ab", "ab", "ab", "ab", "ab", "ab", "ab", "ab" };
            Serializer.Serialize(Stream, fixedList);
            Assert.Matches($"x56{stringType}x98(x02x61x62){{8}}", GetAndReset());
            Serializer.Reset();
            //fixed-length short list
            var fixedShortList = new string[] { "ab", "ab" };
            Serializer.Serialize(Stream, fixedShortList);
            Assert.Matches($"x72{stringType}(x02x61x62){{2}}", GetAndReset());
            Serializer.Reset();

            //fixed-length untyped list
            var fixedUntypedList = new object[] { "ab", "ab", "ab", "ab", "ab", "ab", "ab", "ab" };
            Serializer.Serialize(Stream, fixedUntypedList);
            Assert.Matches($"x58x98(x02x61x62){{8}}", GetAndReset());
            Serializer.Reset();
            //fixed-length short list
            var fixedUntypedShortList = new object[] { "ab", "ab" };
            Serializer.Serialize(Stream, fixedUntypedShortList);
            Assert.Matches($"x7a(x02x61x62){{2}}", GetAndReset());
            Serializer.Reset();
        }

        [Fact]
        public void WriteMap()
        {
            var dict = new Dictionary<int, string> { { 1, "fee" }, { 16, "fie" }, { 256, "foe" } };
            Serializer.Serialize(Stream, dict);
            ResetAndAssert("x48x91x03x66x65x65xa0x03x66x69x65xc9x00x03x66x6fx65x5a");
        }

        [Fact]
        public void WriteObject()
        {
            Serializer.Serialize(Stream, new Car("red", "corvette"));
            ResetAndAssert("x43x14x63x6fx6dx2ex63x61x75x63x68x6fx2ex6dx6fx64x65x6cx2ex43x61x72x92x05x63x6fx6cx6fx72x05x6dx6fx64x65x6cx60x03x72x65x64x08x63x6fx72x76x65x74x74x65");

            Serializer.Serialize(Stream, new Car("green", "civic"));
            ResetAndAssert("x60x05x67x72x65x65x6ex05x63x69x76x69x63");
        }

        [Fact]
        public void WriteEnum()
        {
            Serializer.Serialize(Stream, DayOfWeek.Monday);
            ResetAndAssert("x43x10x53x79x73x74x65x6dx2ex44x61x79x4fx66x57x65x65x6bx91x04x6ex61x6dx65x60x06x4dx6fx6ex64x61x79");

            Serializer.Serialize(Stream, DayOfWeek.Friday);
            ResetAndAssert("x60x06x46x72x69x64x61x79");
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
            Serializer.Serialize(Stream, typeof(System.IO.FileInfo));
            ResetAndAssert("x12x53x79x73x74x65x6dx2ex49x4fx2ex46x69x6cx65x49x6ex66x6f");

            Serializer.Serialize(Stream, typeof(System.String));
            ResetAndAssert("x0dx53x79x73x74x65x6dx2ex53x74x72x69x6ex67");

            Serializer.Serialize(Stream, typeof(System.IO.FileInfo));
            ResetAndAssert("x90");

            Serializer.Serialize(Stream, typeof(System.String));
            ResetAndAssert("x91");
        }
    }
}
