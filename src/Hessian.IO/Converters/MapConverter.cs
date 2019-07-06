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

        public override void WriteValueNotExisted(HessianWriter writer, HessianContext context, object value)
        {
            Type type = value.GetType();
            if (!IsMap(type))
            {
                throw Exceptions.UnExpectedTypeException(type);
            }

            writer.Write(Constants.BC_MAP_UNTYPED);


            var kvType = typeof(KeyValuePair<,>).MakeGenericType(type.GenericTypeArguments);
            var keyProperty = kvType.GetProperty("Key");
            var valueProperty = kvType.GetProperty("Value");
            var keyConverter = AutoConverter.GetConverter(type.GenericTypeArguments[0]);
            var valueConverter = AutoConverter.GetConverter(type.GenericTypeArguments[1]);
            foreach (var entry in (IEnumerable)value)
            {
                keyConverter.WriteValue(writer, context, keyProperty.GetValue(entry));
                valueConverter.WriteValue(writer, context, valueProperty.GetValue(entry));
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
