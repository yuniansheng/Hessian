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

            if (WriteRefIfValueExisted(writer, context, value))
            {
                return;
            }

            if (itemType == typeof(object))
            {
                writer.Write(Constants.BC_LIST_VARIABLE_UNTYPED);
            }
            else
            {
                writer.Write(Constants.BC_LIST_VARIABLE);
                TypeConverter.WriteValue(writer, context, itemType);
            }

            foreach (var item in (IEnumerable)value)
            {
                StringConverter.WriteValue(writer, context, item);
            }
            writer.Write(Constants.BC_END);
        }

        public bool IsList(Type type)
        {
            return type == typeof(ArrayList) || IsGenericList(type);
        }

        private bool IsGenericList(Type type)
        {
            return type.IsGenericType && typeof(List<>).IsAssignableFrom(type.GetGenericTypeDefinition());
        }
    }
}
