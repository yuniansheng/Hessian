using Hessian.IO.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Hessian.IO.Converters
{
    public abstract class HessianConverter
    {
        private static Dictionary<Type, HessianConverter> ConverterCache = new Dictionary<Type, HessianConverter>();

        public static IntConverter IntConverter => GetConverter<IntConverter>();
        public static StringConverter StringConverter => GetConverter<StringConverter>();
        public static TypeConverter TypeConverter => GetConverter<TypeConverter>();
        public static AutoConverter AutoConverter => GetConverter<AutoConverter>();

        public static T GetConverter<T>() where T : HessianConverter
        {
            HessianConverter converter = null;
            var type = typeof(T);

            if (!ConverterCache.TryGetValue(type, out converter))
            {
                converter = Activator.CreateInstance<T>();
                ConverterCache.Add(type, converter);
            }
            return (T)converter;
        }

        public abstract object ReadValue(HessianReader reader, HessianContext context, Type objectType);

        public void WriteValue(HessianWriter writer, HessianContext context, object value)
        {
            if (value == null)
            {
                writer.Write(Constants.BC_NULL);
                return;
            }

            WriteValueNotNull(writer, context, value);
        }

        public abstract void WriteValueNotNull(HessianWriter writer, HessianContext context, object value);
    }
}
