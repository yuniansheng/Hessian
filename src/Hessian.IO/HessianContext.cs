using Hessian.IO.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hessian.IO
{
    public class HessianContext
    {
        public BinaryConverter BinaryConverter { get; }
        public BoolConverter BoolConverter { get; }
        public DateTimeConverter DateTimeConverter { get; }
        public DoubleConverter DoubleConverter { get; }
        public IntConverter IntConverter { get; }
        public LongConverter LongConverter { get; }
        public StringConverter StringConverter { get; }
        public TypeConverter TypeConverter { get; }
        public ObjectConverter ObjectConverter { get; }
        public MapConverter MapConverter { get; }
        public ListConverter ListConverter { get; }

        public ReferenceMap<Type> TypeRefs { get; } = new ReferenceMap<Type>();
        public ReferenceMap<Type> ClassRefs { get; } = new ReferenceMap<Type>();

        public HessianContext()
        {
            BinaryConverter = new BinaryConverter() { Context = this };
            BoolConverter = new BoolConverter() { Context = this };
            DateTimeConverter = new DateTimeConverter() { Context = this };
            DoubleConverter = new DoubleConverter() { Context = this };
            IntConverter = new IntConverter() { Context = this };
            LongConverter = new LongConverter() { Context = this };
            StringConverter = new StringConverter() { Context = this };
            TypeConverter = new TypeConverter() { Context = this };
            ObjectConverter = new ObjectConverter { Context = this };
            MapConverter = new MapConverter { Context = this };
            ListConverter = new ListConverter { Context = this };
        }
    }
}
