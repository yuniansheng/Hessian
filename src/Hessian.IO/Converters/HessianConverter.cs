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

        public static BinaryConverter BinaryConverter => GetConverter<BinaryConverter>();
        public static BoolConverter BoolConverter => GetConverter<BoolConverter>();
        public static DateTimeConverter DateTimeConverter => GetConverter<DateTimeConverter>();
        public static DoubleConverter DoubleConverter => GetConverter<DoubleConverter>();
        public static IntConverter IntConverter => GetConverter<IntConverter>();
        public static LongConverter LongConverter => GetConverter<LongConverter>();
        public static StringConverter StringConverter => GetConverter<StringConverter>();
        public static TypeConverter TypeConverter => GetConverter<TypeConverter>();
        public static ObjectConverter ObjectConverter => GetConverter<ObjectConverter>();
        public static EnumConverter EnumConverter => GetConverter<EnumConverter>();
        public static MapConverter MapConverter => GetConverter<MapConverter>();
        public static ArrayConverter ArrayConverter => GetConverter<ArrayConverter>();
        public static ListConverter ListConverter => GetConverter<ListConverter>();
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

        public abstract void WriteValue(HessianWriter writer, HessianContext context, object value);
    }
}
