using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
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
                context.TypeRefs.AddItem(type);
                return type;
            }
            else if (IntConverter.CanRead(initialOctet))
            {
                var typeIndex = (int)IntConverter.ReadValue(reader, context, typeof(int), initialOctet);
                return context.TypeRefs.GetItem(typeIndex);
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
                else if (elementType == typeof(object))
                {
                    return "[object";
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
                    case "[object":
                        return typeof(object[]);
                    default:
                        typeName = typeName.TrimStart('[');
                        var type = FindTypeInCurrentDomain(typeName);
                        return type.MakeArrayType();
                }
            }
            else
            {
                return FindTypeInCurrentDomain(typeName);
            }
        }

        internal static Type FindTypeInCurrentDomain(string typeName)
        {
            Type type = null;

            //如果该类型已经装载
            type = Type.GetType(typeName);
            if (type != null)
            {
                return type;
            }

            //在EntryAssembly中查找
            if (Assembly.GetEntryAssembly() != null)
            {
                type = Assembly.GetEntryAssembly().GetType(typeName);
                if (type != null)
                {
                    return type;
                }
            }

            //在CurrentDomain的所有Assembly中查找
            Assembly[] assemblyArray = AppDomain.CurrentDomain.GetAssemblies();
            int assemblyArrayLength = assemblyArray.Length;
            for (int i = 0; i < assemblyArrayLength; ++i)
            {
                type = assemblyArray[i].GetType(typeName);
                if (type != null)
                {
                    return type;
                }
            }

            for (int i = 0; (i < assemblyArrayLength); ++i)
            {
                Type[] typeArray = assemblyArray[i].GetTypes();
                int typeArrayLength = typeArray.Length;
                for (int j = 0; j < typeArrayLength; ++j)
                {
                    if (typeArray[j].Name.Equals(typeName))
                    {
                        return typeArray[j];
                    }
                }
            }

            return type;
        }
    }
}
