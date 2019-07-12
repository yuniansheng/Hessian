using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Hessian.IO.Converters
{
    public class ListConverter : ValueRefConverterBase
    {
        public override bool CanRead(byte initialOctet)
        {
            return Constants.BC_LIST_VARIABLE_UNTYPED == initialOctet || Constants.BC_LIST_VARIABLE == initialOctet || base.CanRead(initialOctet);
        }

        public override object ReadValueNotExisted(HessianReader reader, HessianContext context, Type objectType, byte initialOctet)
        {
            Type listType = null;
            Type elementType = null;
            HessianConverter itemConverter = null;
            if (Constants.BC_LIST_VARIABLE_UNTYPED == initialOctet)
            {
                listType = typeof(List<object>);
                elementType = typeof(object);
                itemConverter = AutoConverter;
            }
            else if (Constants.BC_LIST_VARIABLE == initialOctet)
            {

                listType = (Type)TypeConverter.ReadValue(reader, context, null);
                elementType = listType.GenericTypeArguments[0];
                itemConverter = AutoConverter.GetConverter(elementType);
            }
            else
            {
                throw Exceptions.UnExpectedInitialOctet(this, initialOctet);
            }

            var addMethod = listType.GetMethod("Add");
            var list = Activator.CreateInstance(listType);
            context.ValueRefs.AddItem(list);

            initialOctet = reader.ReadByte();
            var parameters = new object[1];
            while (Constants.BC_END != initialOctet)
            {
                parameters[0] = itemConverter.ReadValue(reader, context, elementType, initialOctet);
                addMethod.Invoke(list, parameters);
                initialOctet = reader.ReadByte();
            }
            return list;
        }

        public override void WriteValueNotExisted(HessianWriter writer, HessianContext context, object value)
        {
            Type type = value.GetType();
            Type itemType = null;
            if (type == typeof(ArrayList))
            {
                itemType = typeof(object);
            }
            else if (IsGenericList(type))
            {
                itemType = type.GenericTypeArguments[0];
            }
            else
            {
                throw Exceptions.UnExpectedTypeException(type);
            }

            HessianConverter itemConverter = null;
            if (itemType == typeof(object))
            {
                writer.Write(Constants.BC_LIST_VARIABLE_UNTYPED);
                itemConverter = AutoConverter;
            }
            else
            {
                writer.Write(Constants.BC_LIST_VARIABLE);
                TypeConverter.WriteType(writer, context, type);
                itemConverter = AutoConverter.GetConverter(itemType);
            }

            foreach (var item in (IEnumerable)value)
            {
                itemConverter.WriteValue(writer, context, item);
            }
            writer.Write(Constants.BC_END);
        }

        public static bool IsList(Type type)
        {
            return type == typeof(ArrayList) || IsGenericList(type);
        }

        private static bool IsGenericList(Type type)
        {
            return type.IsGenericType && typeof(List<>).IsAssignableFrom(type.GetGenericTypeDefinition());
        }
    }
}
