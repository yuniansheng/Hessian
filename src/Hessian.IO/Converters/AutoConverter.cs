using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Hessian.IO.Converters
{
    public class AutoConverter : HessianConverter
    {
        public override object ReadValue(HessianReader reader, HessianContext context, Type objectType)
        {
            throw new NotImplementedException();
        }

        public override void WriteValue(HessianWriter writer, HessianContext context, object value)
        {
            if (value == null)
            {
                writer.Write(Constants.BC_NULL);
                return;
            }

            var type = value.GetType();
            var converter = GetConverter(type);
            converter.WriteValue(writer, context, value);
        }

        private HessianConverter GetConverter(Type type)
        {
            if (type.IsPrimitive)
            {
                var typeCode = Type.GetTypeCode(type);
                switch (typeCode)
                {
                    case TypeCode.Boolean:
                        return BoolConverter;
                    case TypeCode.Byte:
                    case TypeCode.SByte:
                    case TypeCode.Int16:
                    case TypeCode.UInt16:
                    case TypeCode.Int32:
                    case TypeCode.UInt32:
                        return IntConverter;
                    case TypeCode.Double:
                    case TypeCode.Single:
                        return DoubleConverter;
                    case TypeCode.Int64:
                    case TypeCode.UInt64:
                        return LongConverter;
                    case TypeCode.Char:
                        return StringConverter;
                    default:
                        return ObjectConverter;
                }
            }
            else if (type == typeof(string))
            {
                return StringConverter;
            }
            else if (type == typeof(DateTime))
            {
                return DateTimeConverter;
            }
            else if (type.IsArray)
            {
                return ArrayConverter;
            }
            else if (type.IsEnum)
            {
                return EnumConverter;
            }
            else if (type.FullName == "System.RuntimeType")
            {
                return TypeConverter;
            }
            else if (ListConverter.IsList(type))
            {
                return ListConverter;
            }
            else if (MapConverter.IsMap(type))
            {
                return MapConverter;
            }
            else
            {
                return ObjectConverter;
            }
        }
    }
}
