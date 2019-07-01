using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Hessian.IO.Converters
{
    public class TypeConverter : HessianConverter
    {
        private ReferenceMap<Type> _typeRefs = new ReferenceMap<Type>();

        public override object ReadValue(HessianReader reader, Type objectType)
        {
            throw new NotImplementedException();
        }

        public override void WriteValue(HessianWriter writer, object value)
        {
            Type t = (Type)value;

            (int index, bool isNewItem) = _typeRefs.AddItem(t);

            if (isNewItem)
            {
                Context.StringConverter.WriteValue(writer, t.FullName);
            }
            else
            {
                Context.IntConverter.WriteValue(writer, index);
            }
        }
    }
}
