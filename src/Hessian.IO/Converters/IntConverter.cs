using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Hessian.IO.Converters
{
    public class IntConverter : HessianConverter
    {
        public override object ReadValue(HessianReader reader, Type objectType)
        {
            throw new NotImplementedException();
        }

        public override void WriteValue(HessianWriter writer, object value)
        {
            var type = value.GetType();
            var typeCode = Type.GetTypeCode(type);
            switch (typeCode)
            {
                case TypeCode.Byte:
                    WriteInt(writer, (byte)value);
                    break;
                case TypeCode.SByte:
                    WriteInt(writer, (sbyte)value);
                    break;
                case TypeCode.Int16:
                    WriteInt(writer, (short)value);
                    break;
                case TypeCode.UInt16:
                    WriteInt(writer, (ushort)value);
                    break;
                case TypeCode.Int32:
                    WriteInt(writer, (int)value);
                    break;
                case TypeCode.UInt32:
                    WriteUInt(writer, (uint)value);
                    break;
                default:
                    throw Exceptions.UnExpectedTypeException(type);
            }
        }

        public void WriteInt(HessianWriter writer, int value)
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

        public void WriteUInt(HessianWriter writer, uint value)
        {
            WriteInt(writer, unchecked((int)value));
        }
    }
}
