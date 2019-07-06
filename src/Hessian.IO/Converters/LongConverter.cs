using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Hessian.IO.Converters
{
    public class LongConverter : HessianConverter
    {
        public override object ReadValue(HessianReader reader, HessianContext context, Type objectType)
        {
            throw new NotImplementedException();
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
