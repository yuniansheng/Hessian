﻿using Hessian.IO.Converters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Xunit;

namespace Hessian.IO.Test.Converters
{
    public class ConverterWriteTest : ConverterTestBase
    {
        public BinaryConverter BinaryConverter => new BinaryConverter();
        public BoolConverter BoolConverter => new BoolConverter();
        public DateTimeConverter DateTimeConverter => new DateTimeConverter();
        public DoubleConverter DoubleConverter => new DoubleConverter();
        public IntConverter IntConverter => new IntConverter();
        public LongConverter LongConverter => new LongConverter();
        public StringConverter StringConverter => new StringConverter();

        [Fact]
        public void WriteBinary()
        {
            BinaryConverter.WriteValue(Writer, new byte[0]);
            Assert.Matches("x20", GetAndReset());

            BinaryConverter.WriteValue(Writer, new byte[15]);
            Assert.Matches("x2f(x00){15}", GetAndReset());

            BinaryConverter.WriteValue(Writer, new byte[1024 * 4 + 15]);
            Assert.Matches("x62x10x00(x00){4096}x2f(x00){15}", GetAndReset());

            BinaryConverter.WriteValue(Writer, new byte[1024 * 4 + 16]);
            Assert.Matches("x62x10x00(x00){4096}x42x00x10(x00){16}", GetAndReset());
        }

        [Fact]
        public void WriteBool()
        {
            BoolConverter.WriteValue(Writer, true);
            ResetAndAssert("x54");

            BoolConverter.WriteValue(Writer, false);
            ResetAndAssert("x46");
        }

        [Fact]
        public void WriteDateTime()
        {
            DateTimeConverter.WriteValue(Writer, new DateTime(1998, 5, 8, 9, 51, 31, DateTimeKind.Utc));
            ResetAndAssert("x4ax00x00x00xd0x4bx92x84xb8");

            DateTimeConverter.WriteValue(Writer, new DateTime(1998, 5, 8, 9, 51, 0, DateTimeKind.Utc));
            ResetAndAssert("x4bx00xe3x83x8f");
        }

        [Fact]
        public void WriteDouble()
        {
            DoubleConverter.WriteValue(Writer, 0d);
            ResetAndAssert("x5b");

            DoubleConverter.WriteValue(Writer, 1d);
            ResetAndAssert("x5c");

            DoubleConverter.WriteValue(Writer, -128d);
            ResetAndAssert("x5dx80");

            DoubleConverter.WriteValue(Writer, 127d);
            ResetAndAssert("x5dx7f");

            DoubleConverter.WriteValue(Writer, -32768d);
            ResetAndAssert("x5ex80x00");

            DoubleConverter.WriteValue(Writer, 32767d);
            ResetAndAssert("x5ex7fxff");

            DoubleConverter.WriteValue(Writer, 12.5d);
            ResetAndAssert("x5fx00x00x30xd4");

            DoubleConverter.WriteValue(Writer, 12.123d);
            ResetAndAssert("x44x40x28x3exf9xdbx22xd0xe5");
        }

        [Fact]
        public void WriteInt()
        {
            IntConverter.WriteValue(Writer, -16);
            ResetAndAssert("x80");

            IntConverter.WriteValue(Writer, 47);
            ResetAndAssert("xbf");

            IntConverter.WriteValue(Writer, -2048);
            ResetAndAssert("xc0x00");

            IntConverter.WriteValue(Writer, 2047);
            ResetAndAssert("xcfxff");

            IntConverter.WriteValue(Writer, -262144);
            ResetAndAssert("xd0x00x00");

            IntConverter.WriteValue(Writer, 262143);
            ResetAndAssert("xd7xffxff");

            IntConverter.WriteValue(Writer, int.MinValue);
            ResetAndAssert("x49x80x00x00x00");

            IntConverter.WriteValue(Writer, int.MaxValue);
            ResetAndAssert("x49x7fxffxffxff");
        }

        [Fact]
        public void WriteLong()
        {
            LongConverter.WriteValue(Writer, -8L);
            ResetAndAssert("xd8");

            LongConverter.WriteValue(Writer, 15L);
            ResetAndAssert("xef");

            LongConverter.WriteValue(Writer, -2048L);
            ResetAndAssert("xf0x00");

            LongConverter.WriteValue(Writer, 2047L);
            ResetAndAssert("xffxff");

            LongConverter.WriteValue(Writer, -262144L);
            ResetAndAssert("x38x00x00");

            LongConverter.WriteValue(Writer, 262143L);
            ResetAndAssert("x3fxffxff");

            LongConverter.WriteValue(Writer, (long)int.MinValue);
            ResetAndAssert("x59x80x00x00x00");

            LongConverter.WriteValue(Writer, (long)int.MaxValue);
            ResetAndAssert("x59x7fxffxffxff");

            LongConverter.WriteValue(Writer, long.MinValue);
            ResetAndAssert("x4cx80x00x00x00x00x00x00x00");

            LongConverter.WriteValue(Writer, long.MaxValue);
            ResetAndAssert("x4cx7fxffxffxffxffxffxffxff");
        }

        [Fact]
        public void WriteString()
        {
            StringConverter.WriteValue(Writer, string.Empty);
            Assert.Matches("x00", GetAndReset());

            StringConverter.WriteValue(Writer, new string('a', 31));
            Assert.Matches("x1f(x61){31}", GetAndReset());

            StringConverter.WriteValue(Writer, new string('a', 1023));
            Assert.Matches("x33xff(x61){1023}", GetAndReset());

            StringConverter.WriteValue(Writer, new string('a', 1024 * 4));
            Assert.Matches("x53x10xff(x61){4096}", GetAndReset());

            StringConverter.WriteValue(Writer, new string('a', 1024 * 4 + 31));
            Assert.Matches("x53x10xff(x61){4096}x1f(x61){31}", GetAndReset());
        }
    }
}
