using Hessian.IO.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hessian.IO
{
    public class HessianContext
    {
        private Dictionary<Type, HessianConverter> _converterCache = new Dictionary<Type, HessianConverter>();

        public BinaryConverter BinaryConverter => GetConverter<BinaryConverter>();
        public BoolConverter BoolConverter => GetConverter<BoolConverter>();
        public DateTimeConverter DateTimeConverter => GetConverter<DateTimeConverter>();
        public DoubleConverter DoubleConverter => GetConverter<DoubleConverter>();
        public IntConverter IntConverter => GetConverter<IntConverter>();
        public LongConverter LongConverter => GetConverter<LongConverter>();
        public StringConverter StringConverter => GetConverter<StringConverter>();
        public TypeConverter TypeConverter => GetConverter<TypeConverter>();
        public ObjectConverter ObjectConverter => GetConverter<ObjectConverter>();
        public EnumConverter EnumConverter => GetConverter<EnumConverter>();
        public MapConverter MapConverter => GetConverter<MapConverter>();
        public ArrayConverter ArrayConverter => GetConverter<ArrayConverter>();
        public ListConverter ListConverter => GetConverter<ListConverter>();
        public AutoConverter AutoConverter => GetConverter<AutoConverter>();

        public ReferenceMap<Type> TypeRefs { get; set; } = new ReferenceMap<Type>();
        public ReferenceMap<Type> ClassRefs { get; set; } = new ReferenceMap<Type>();

        public HessianContext()
        {
        }

        public T GetConverter<T>() where T : HessianConverter
        {
            HessianConverter converter = null;
            var type = typeof(T);

            if (!_converterCache.TryGetValue(typeof(T), out converter))
            {
                converter = Activator.CreateInstance<T>();
                type.GetProperty(nameof(HessianConverter.Context)).SetValue(converter, this);
                _converterCache.Add(typeof(T), converter);
            }
            return (T)converter;
        }
    }
}
