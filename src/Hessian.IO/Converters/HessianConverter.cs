using Hessian.IO.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Hessian.IO.Converters
{
    public abstract class HessianConverter
    {
        protected static Dictionary<Type, HessianConverter> ConverterCache = new Dictionary<Type, HessianConverter>();

        static HessianConverter()
        {
            var types = Assembly.GetAssembly(typeof(HessianConverter)).GetTypes()
                .Where(t => !t.IsAbstract && typeof(HessianConverter).IsAssignableFrom(t));

            foreach (var t in types)
            {
                var converter = (HessianConverter)Activator.CreateInstance(t);
                ConverterCache.Add(t, converter);
            }
        }

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

        public virtual bool CanRead(byte initialOctet)
        {
            return false;
        }

        public virtual object ReadValue(HessianReader reader, HessianContext context, Type objectType)
        {
            var initialOctet = reader.ReadByte();
            if (initialOctet == Constants.BC_NULL)
            {
                return null;
            }

            return ReadValue(reader, context, objectType, initialOctet);
        }

        public virtual object ReadValue(HessianReader reader, HessianContext context, Type objectType, byte initialOctet) { return null; }

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
