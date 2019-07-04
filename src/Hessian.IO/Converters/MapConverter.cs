using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Text;

namespace Hessian.IO.Converters
{
    public class MapConverter : ValueRefConverterBase
    {
        public override object ReadValue(HessianReader reader, HessianContext context, Type objectType)
        {
            throw new NotImplementedException();
        }

        public override void WriteValue(HessianWriter writer, HessianContext context, object value)
        {
            Type type = value.GetType();
            if (!IsMap(type))
            {
                throw Exceptions.UnExpectedTypeException(type);
            }

            if (WriteRefIfValueExisted(writer, context, value))
            {
                return;
            }

            writer.Write(Constants.BC_MAP_UNTYPED);


            var kvType = typeof(KeyValuePair<,>).MakeGenericType(type.GetGenericArguments());
            var keyProperty = kvType.GetProperty("Key");
            var valueProperty = kvType.GetProperty("Value");
            foreach (var entry in (IEnumerable)value)
            {
                IntConverter.WriteValue(writer, context, keyProperty.GetValue(entry));
                StringConverter.WriteValue(writer, context, valueProperty.GetValue(entry));
            }

            writer.Write(Constants.BC_END);
        }

        public bool IsMap(Type type)
        {
            if (type.IsGenericType && type.GenericTypeArguments.Length == 2)
            {
                var genericTypeDefinition = type.GetGenericTypeDefinition();
                if (typeof(Dictionary<,>).IsAssignableFrom(genericTypeDefinition) || typeof(ReadOnlyDictionary<,>).IsAssignableFrom(genericTypeDefinition))
                {
                    return true;
                }

                if (typeof(IDictionary<,>).MakeGenericType(type.GenericTypeArguments).IsAssignableFrom(type) || typeof(IReadOnlyDictionary<,>).MakeGenericType(type.GenericTypeArguments).IsAssignableFrom(type))
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
