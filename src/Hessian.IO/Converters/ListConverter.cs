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
        public override object ReadValue(HessianReader reader, HessianContext context, Type objectType)
        {
            throw new NotImplementedException();
        }

        public override void WriteValue(HessianWriter writer, HessianContext context, object value)
        {
            Type type = value.GetType();
            Type elementType = null;
            HessianConverter itemConverter = null;
            if (type == typeof(ArrayList))
            {
                elementType = typeof(object);

            }
            else if (IsGenericList(type))
            {
                elementType = type.GenericTypeArguments[0];
            }
            else
            {
                throw Exceptions.UnExpectedTypeException(type);
            }

            if (WriteRefIfValueExisted(writer, context, value))
            {
                return;
            }

            if (elementType == typeof(object))
            {
                itemConverter = AutoConverter;
                writer.Write(Constants.BC_LIST_VARIABLE_UNTYPED);
            }
            else
            {
                itemConverter = AutoConverter.GetConverter(elementType);
                writer.Write(Constants.BC_LIST_VARIABLE);
                TypeConverter.WriteValue(writer, context, elementType);
            }

            foreach (var item in (IEnumerable)value)
            {
                itemConverter.WriteValue(writer, context, item);
            }
            writer.Write(Constants.BC_END);
        }

        public bool IsList(Type type)
        {
            return type == typeof(ArrayList) || IsGenericList(type);
        }

        private bool IsGenericList(Type type)
        {
            if (type.IsGenericType && type.GenericTypeArguments.Length == 1)
            {
                var genericTypeDefinition = type.GetGenericTypeDefinition();
                if (typeof(List<>).IsAssignableFrom(genericTypeDefinition))
                {
                    return true;
                }

                if (typeof(IList<>).MakeGenericType(type.GenericTypeArguments).IsAssignableFrom(type))
                {
                    return true;
                }
            }
            {
                return false;
            }
        }
    }
}
