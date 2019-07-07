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
                TypeConverter.WriteValueNotNull(writer, context, type);
                itemConverter = AutoConverter.GetConverter(itemType);
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
            return type.IsGenericType && typeof(List<>).IsAssignableFrom(type.GetGenericTypeDefinition());
        }
    }
}
