using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Hessian.IO.Converters
{
    public class TypeConverter : HessianConverter
    {
        public override bool CanRead(byte initialOctet)
        {
            return StringConverter.CanRead(initialOctet) || IntConverter.CanRead(initialOctet);
        }

        public override object ReadValue(HessianReader reader, HessianContext context, Type objectType, byte initialOctet)
        {
            if (StringConverter.CanRead(initialOctet))
            {
                var typeName = (string)StringConverter.ReadValue(reader, context, typeof(string), initialOctet);
                var type = GetType(typeName);
                context.ClassRefs.AddItem(type);
                return type;
            }
            else if (IntConverter.CanRead(initialOctet))
            {
                var typeIndex = (int)IntConverter.ReadValue(reader, context, typeof(int), initialOctet);
                return context.ClassRefs.GetItem(typeIndex);
            }
            else
            {
                throw Exceptions.UnExpectedInitialOctet(this, initialOctet);
            }
        }

        public override void WriteValueNotNull(HessianWriter writer, HessianContext context, object value)
        {
            if (!(value is Type))
            {
                throw Exceptions.UnExpectedTypeException(value.GetType());
            }

            WriteType(writer, context, (Type)value);
        }

        public void WriteType(HessianWriter writer, HessianContext context, Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            (int index, bool isNewItem) = context.TypeRefs.AddItem(type);

            if (isNewItem)
            {
                StringConverter.WriteValueNotNull(writer, context, GetTypeName(type));
            }
            else
            {
                IntConverter.WriteInt(writer, context, index);
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

        private Type GetType(string typeName)
        {
            if (typeName.StartsWith("["))
            {
                switch (typeName)
                {
                    case "[boolean":
                        return typeof(bool[]);
                    case "[short":
                        return typeof(short[]);
                    case "[int":
                        return typeof(int[]);
                    case "[double":
                        return typeof(double[]);
                    case "[float":
                        return typeof(float[]);
                    case "[long":
                        return typeof(long[]);
                    case "[string":
                        return typeof(string[]);
                    default:
                        typeName = typeName.TrimStart('[');
                        var type = Type.GetType(typeName);
                        return type.MakeArrayType();
                }
            }
            else
            {
                return Type.GetType(typeName);
            }
        }
    }
}
