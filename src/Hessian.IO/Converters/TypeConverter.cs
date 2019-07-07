using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Hessian.IO.Converters
{
    public class TypeConverter : HessianConverter
    {
        public override object ReadValue(HessianReader reader, HessianContext context, Type objectType)
        {
            throw new NotImplementedException();
        }

        public override void WriteValueNotNull(HessianWriter writer, HessianContext context, object value)
        {
            if (!(value is Type))
            {
                throw Exceptions.UnExpectedTypeException(value.GetType());
            }

            var type = (Type)value;

            (int index, bool isNewItem) = context.TypeRefs.AddItem(type);

            if (isNewItem)
            {
                StringConverter.WriteValueNotNull(writer, context, GetTypeName(type));
            }
            else
            {
                IntConverter.WriteValueNotNull(writer, context, index);
            }
        }

        public string GetTypeName(Type type)
        {
            if (type.IsArray)
            {
                var elementType = type.GetElementType();
                if (elementType.IsPrimitive)
                {
                    var typeCode = Type.GetTypeCode(elementType);
                    switch (typeCode)
                    {
                        case TypeCode.Boolean:
                            return "[boolean";
                        case TypeCode.Int16:
                        case TypeCode.UInt16:
                            return "[short";
                        case TypeCode.Int32:
                        case TypeCode.UInt32:
                            return "[int";
                        case TypeCode.Double:
                            return "[double";
                        case TypeCode.Single:
                            return "[float";
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                            return "[long";
                        default:
                            return "[" + elementType.ToString();
                    }
                }
                else if (elementType == typeof(string))
                {
                    return "[string";
                }
                else
                {
                    return "[" + elementType.ToString();
                }
            }
            return type.ToString();
        }
    }
}
