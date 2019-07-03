using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Hessian.IO.Converters
{
    public class AutoConverter : HessianConverter
    {
        public override object ReadValue(HessianReader reader, Type objectType)
        {
            throw new NotImplementedException();
        }

        public override void WriteValue(HessianWriter writer, object value)
        {
            if (value == null)
            {
                writer.Write(Constants.BC_NULL);
                return;
            }

            var t = value.GetType();
            var converter = GetConverter(t);
            converter.WriteValue(writer, value);
        }

        private HessianConverter GetConverter(Type type)
        {
            if (type.IsPrimitive)
            {
                var typeCode = Type.GetTypeCode(type);
                switch (typeCode)
                {
                    case TypeCode.Boolean:
                        return Context.BoolConverter;
                    case TypeCode.Byte:
                    case TypeCode.SByte:
                    case TypeCode.Int16:
                    case TypeCode.UInt16:
                    case TypeCode.Int32:
                    case TypeCode.UInt32:
                        return Context.IntConverter;
                    case TypeCode.Double:
                    case TypeCode.Single:
                        return Context.DoubleConverter;
                    case TypeCode.Int64:
                    case TypeCode.UInt64:
                        return Context.LongConverter;
                    case TypeCode.Char:
                        return Context.StringConverter;
                    default:
                        return Context.ObjectConverter;
                }
            }
            else if (type == typeof(string))
            {
                return Context.StringConverter;
            }
            else if (type == typeof(DateTime))
            {
                return Context.DateTimeConverter;
            }
            else if (type.IsArray)
            {
                return Context.ArrayConverter;
            }
            else if (type.IsEnum)
            {
                return Context.EnumConverter;
            }
            else if (Context.ListConverter.IsList(type))
            {
                return Context.ListConverter;
            }
            else if (Context.MapConverter.IsMap(type))
            {
                return Context.MapConverter;
            }
            else
            {
                return Context.ObjectConverter;
            }
        }
    }
}
