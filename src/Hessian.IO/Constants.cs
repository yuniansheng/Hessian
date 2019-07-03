using System;
using System.Collections.Generic;
using System.Text;

namespace Hessian.IO
{
    public class Constants
    {
        public const byte BC_BINARY = (byte)'B'; // final chunk
        public const byte BC_BINARY_CHUNK = (byte)'b'; // non-final chunk
        public const byte BC_BINARY_DIRECT = 0x20; // 1-byte length binary
        public const int BINARY_DIRECT_MAX = 0x0f;
        public const int BINARY_CHUNK_SIZE = 1024 * 4; //chunk size is 4k

        public const byte BC_DATE = 0x4a;   // 64-bit millisecond UTC date
        public const byte BC_DATE_MINUTE = 0x4b;    // 32-bit minute UTC date

        public const byte BC_DOUBLE = (byte)'D'; // IEEE 64-bit double
        public const byte BC_DOUBLE_ZERO = 0x5b;
        public const byte BC_DOUBLE_ONE = 0x5c;
        public const byte BC_DOUBLE_BYTE = 0x5d;
        public const byte BC_DOUBLE_SHORT = 0x5e;
        public const byte BC_DOUBLE_MILL = 0x5f;

        public const byte BC_INT = (byte)'I'; // 32-bit int
        public const int INT_DIRECT_MIN = -0x10;
        public const int INT_DIRECT_MAX = 0x2f;
        public const byte BC_INT_ZERO = 0x90;
        public const int INT_BYTE_MIN = -0x800;
        public const int INT_BYTE_MAX = 0x7ff;
        public const byte BC_INT_BYTE_ZERO = 0xc8;
        public const int INT_SHORT_MIN = -0x40000;
        public const int INT_SHORT_MAX = 0x3ffff;
        public const byte BC_INT_SHORT_ZERO = 0xd4;

        public const byte BC_LIST_VARIABLE = 0x55;
        public const byte BC_LIST_VARIABLE_UNTYPED = 0x57;
        public const byte BC_LIST_FIXED = (byte)'V';
        public const byte BC_LIST_FIXED_UNTYPED = 0x58;
        public const byte BC_LIST_DIRECT = 0x70;
        public const byte BC_LIST_DIRECT_UNTYPED = 0x78;
        public const int LIST_DIRECT_MAX = 0x7;

        public const byte BC_LONG = (byte)'L'; // 64-bit signed integer
        public const int LONG_DIRECT_MIN = -0x08;
        public const int LONG_DIRECT_MAX = 0x0f;
        public const byte BC_LONG_ZERO = 0xe0;
        public const int LONG_BYTE_MIN = -0x800;
        public const int LONG_BYTE_MAX = 0x7ff;
        public const byte BC_LONG_BYTE_ZERO = 0xf8;
        public const int LONG_SHORT_MIN = -0x40000;
        public const int LONG_SHORT_MAX = 0x3ffff;
        public const byte BC_LONG_SHORT_ZERO = 0x3c;
        public const int LONG_INT_MIN = -0x80000000;
        public const int LONG_INT_MAX = 0x7fffffff;
        public const byte BC_LONG_INT = 0x59;

        public const byte BC_MAP = (byte)'M';
        public const byte BC_MAP_UNTYPED = (byte)'H';

        public const byte BC_NULL = (byte)'N';

        public const byte BC_OBJECT = (byte)'O';
        public const byte BC_OBJECT_DEF = (byte)'C';
        public const byte BC_OBJECT_DIRECT = 0x60;
        public const int OBJECT_DIRECT_MAX = 0x0f;

        public const byte BC_STRING = (byte)'S'; // final string
        public const byte BC_STRING_CHUNK = (byte)'R'; // non-final string
        public const int STRING_CHUNK_SIZE = 1024 * 4;
        public const byte BC_STRING_DIRECT = 0x00;
        public const int STRING_DIRECT_MAX = 0x1f;
        public const byte BC_STRING_SHORT = 0x30;
        public const int STRING_SHORT_MAX = 0x3ff;

        public const byte BC_TRUE = (byte)'T';
        public const byte BC_FALSE = (byte)'F';

        public const byte BC_END = (byte)'Z';
    }
}
