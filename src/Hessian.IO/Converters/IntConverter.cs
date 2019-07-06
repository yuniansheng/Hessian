using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Hessian.IO.Converters
{
    public class IntConverter : HessianConverter
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
                case TypeCode.Byte:
                    WriteInt(writer, context, (byte)value);
                    break;
                case TypeCode.SByte:
                    WriteInt(writer, context, (sbyte)value);
                    break;
                case TypeCode.Int16:
                    WriteInt(writer, context, (short)value);
                    break;
                case TypeCode.UInt16:
                    WriteInt(writer, context, (ushort)value);
                    break;
                case TypeCode.Int32:
                    WriteInt(writer, context, (int)value);
                    break;
                case TypeCode.UInt32:
                    WriteUInt(writer, context, (uint)value);
                    break;
                default:
                    throw Exceptions.UnExpectedTypeException(type);
            }
        }

        public void WriteInt(HessianWriter writer, HessianContext context, int value)
        {
            if (Constants.INT_DIRECT_MIN <= value && value <= Constants.INT_DIRECT_MAX)
                writer.Write((byte)(Constants.BC_INT_ZERO + value));
            else if (Constants.INT_BYTE_MIN <= value && value <= Constants.INT_BYTE_MAX)
            {
                writer.Write((byte)(Constants.BC_INT_BYTE_ZERO + (value >> 8)));
                writer.Write((byte)value);
            }
            else if (Constants.INT_SHORT_MIN <= value && value <= Constants.INT_SHORT_MAX)
            {
                writer.Write((byte)(Constants.BC_INT_SHORT_ZERO + (value >> 16)));
                writer.Write((short)value);
            }
            else
            {
                writer.Write(Constants.BC_INT);
                writer.Write(value);
            }
        }

        public void WriteUInt(HessianWriter writer, HessianContext context, uint value)
        {
            WriteInt(writer, context, unchecked((int)value));
        }
    }
}
