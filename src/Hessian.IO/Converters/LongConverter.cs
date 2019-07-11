using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Hessian.IO.Converters
{
    public class LongConverter : HessianConverter
    {
        public override bool CanRead(byte initialOctet)
        {
            return (0xd8 <= initialOctet && initialOctet <= 0xef) ||
                (0xf0 <= initialOctet && initialOctet <= 0xff) ||
                (0x38 <= initialOctet && initialOctet <= 0x3f) ||
                Constants.BC_LONG_INT == initialOctet || Constants.BC_LONG == initialOctet;
        }

        public override object ReadValue(HessianReader reader, HessianContext context, Type objectType)
        {
            throw new NotImplementedException();
        }

        public override object ReadValue(HessianReader reader, HessianContext context, Type objectType, byte initialOctet)
        {
            if (0xd8 <= initialOctet && initialOctet <= 0xef)
            {
                return (long)(initialOctet - 0xe0);
            }
            else if (0xf0 <= initialOctet && initialOctet <= 0xff)
            {
                var b0 = reader.ReadByte();
                return (long)(((initialOctet - 0xf8) << 8) + b0);
            }
            else if (0x38 <= initialOctet && initialOctet <= 0x3f)
            {
                var s = reader.ReadUInt16();
                return (long)(((initialOctet - 0x3c) << 16) + s);
            }
            else if (Constants.BC_LONG_INT == initialOctet)
            {
                return (long)reader.ReadInt32();
            }
            else if (Constants.BC_LONG == initialOctet)
            {
                return reader.ReadInt64();
            }
            else
            {
                throw Exceptions.UnExpectedInitialOctet(this, initialOctet);
            }
        }

        public override void WriteValueNotNull(HessianWriter writer, HessianContext context, object value)
        {
            var type = value.GetType();
            var typeCode = Type.GetTypeCode(type);
            switch (typeCode)
            {
                case TypeCode.Int64:
                    WriteLong(writer, (long)value);
                    break;
                case TypeCode.UInt64:
                    WriteULong(writer, (ulong)value);
                    break;
                default:
                    throw Exceptions.UnExpectedTypeException(type);
            }
        }

        public void WriteLong(HessianWriter writer, long value)
        {
            if (Constants.LONG_DIRECT_MIN <= value && value <= Constants.LONG_DIRECT_MAX)
            {
                writer.Write((byte)(Constants.BC_LONG_ZERO + value));
            }
            else if (Constants.LONG_BYTE_MIN <= value && value <= Constants.LONG_BYTE_MAX)
            {
                writer.Write((byte)(Constants.BC_LONG_BYTE_ZERO + (value >> 8)));
                writer.Write((byte)value);
            }
            else if (Constants.LONG_SHORT_MIN <= value && value <= Constants.LONG_SHORT_MAX)
            {
                writer.Write((byte)(Constants.BC_LONG_SHORT_ZERO + (value >> 16)));
                writer.Write((short)value);
            }
            else if (Constants.LONG_INT_MIN <= value && value <= Constants.LONG_INT_MAX)
            {
                writer.Write((byte)(Constants.BC_LONG_INT));
                writer.Write((int)value);
            }
            else
            {
                writer.Write(Constants.BC_LONG);
                writer.Write(value);
            }
        }

        public void WriteULong(HessianWriter writer, ulong value)
        {
            WriteLong(writer, unchecked((long)value));
        }
    }
}
