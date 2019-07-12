using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Hessian.IO.Converters
{
    public class AutoConverter : HessianConverter
    {
        public override bool CanRead(byte initialOctet)
        {
            return true;
        }

        public override object ReadValue(HessianReader reader, HessianContext context, Type objectType, byte initialOctet)
        {
            if (Constants.BC_REF == initialOctet)
            {
                var index = (int)IntConverter.ReadValue(reader, context, typeof(int));
                return context.ValueRefs.GetItem(index);
            }

            foreach (var converter in ConverterCache.Values)
            {
                if (converter != this && !(converter is ClassDefinitionConverter) && converter.CanRead(initialOctet))
                {
                    return converter.ReadValue(reader, context, objectType, initialOctet);
                }
            }

            throw Exceptions.UnExpectedInitialOctet(this, initialOctet);
        }

        public override void WriteValueNotNull(HessianWriter writer, HessianContext context, object value)
        {
            var type = value.GetType();
            var converter = GetConverter(type);
            converter.WriteValueNotNull(writer, context, value);
        }

        public HessianConverter GetConverter(Type type)
        {
            if (type.IsPrimitive)
            {
                var typeCode = Type.GetTypeCode(type);
                switch (typeCode)
                {
                    case TypeCode.Boolean:
                        return GetConverter<BoolConverter>();
                    case TypeCode.Byte:
                    case TypeCode.SByte:
                    case TypeCode.Int16:
                    case TypeCode.UInt16:
                    case TypeCode.Int32:
                    case TypeCode.UInt32:
                        return IntConverter;
                    case TypeCode.Double:
                    case TypeCode.Single:
                        return GetConverter<DoubleConverter>();
                    case TypeCode.Int64:
                    case TypeCode.UInt64:
                        return GetConverter<LongConverter>();
                    case TypeCode.Char:
                        return StringConverter;
                    default:
                        return GetConverter<ObjectConverter>();
                }
            }
            else if (type == typeof(string))
            {
                return StringConverter;
            }
            else if (type == typeof(DateTime))
            {
                return GetConverter<DateTimeConverter>();
            }
            else if (type.IsArray)
            {
                return GetConverter<ArrayConverter>();
            }
            else if (type.FullName == "System.RuntimeType")
            {
                return TypeConverter;
            }
            else if (ListConverter.IsList(type))
            {
                return GetConverter<ListConverter>();
            }
            else if (MapConverter.IsMap(type))
            {
                return GetConverter<MapConverter>();
            }
            else
            {
                return GetConverter<ObjectConverter>();
            }
        }
    }
}
