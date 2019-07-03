using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Hessian.IO.Converters
{
    public class EnumConverter : HessianConverter
    {
        public override object ReadValue(HessianReader reader, Type objectType)
        {
            throw new NotImplementedException();
        }

        public override void WriteValue(HessianWriter writer, object value)
        {
            Type t = value.GetType();
            if (!t.IsEnum)
            {
                throw Exceptions.UnExpectedTypeException(t);
            }

            (var index, var isNewItem) = Context.ClassRefs.AddItem(t);

            if (isNewItem)
            {
                WriteClassDefinition(writer, t);
            }

            if (index <= Constants.OBJECT_DIRECT_MAX)
            {
                writer.Write((byte)(Constants.BC_OBJECT_DIRECT + index));
            }
            else
            {
                writer.Write(Constants.BC_OBJECT);
                writer.Write(index);
            }

            if (Enum.IsDefined(t, value))
            {
                Context.StringConverter.WriteValue(writer, value.ToString());
            }
            else
            {
                var underlyingTypeCode = Type.GetTypeCode(t);
                switch (underlyingTypeCode)
                {
                    case TypeCode.Byte:
                        Context.IntConverter.WriteInt(writer, (byte)value);
                        break;
                    case TypeCode.SByte:
                        Context.IntConverter.WriteInt(writer, (sbyte)value);
                        break;
                    case TypeCode.Int16:
                        Context.IntConverter.WriteInt(writer, (short)value);
                        break;
                    case TypeCode.UInt16:
                        Context.IntConverter.WriteInt(writer, (ushort)value);
                        break;
                    case TypeCode.Int32:
                        Context.IntConverter.WriteInt(writer, (int)value);
                        break;
                    case TypeCode.UInt32:
                        Context.IntConverter.WriteUInt(writer, (uint)value);
                        break;
                    case TypeCode.Int64:
                        Context.LongConverter.WriteLong(writer, (long)value);
                        break;
                    case TypeCode.UInt64:
                        Context.LongConverter.WriteULong(writer, (ulong)value);
                        break;
                    default:
                        break;
                }
            }
        }

        private void WriteClassDefinition(HessianWriter writer, Type type)
        {
            writer.Write(Constants.BC_OBJECT_DEF);
            Context.StringConverter.WriteValue(writer, type.FullName);

            Context.IntConverter.WriteValue(writer, 1);
            Context.StringConverter.WriteValue(writer, "name");
        }
    }
}
