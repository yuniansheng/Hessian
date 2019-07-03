using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Hessian.IO.Converters
{
    public class TypeConverter : HessianConverter
    {
        public override object ReadValue(HessianReader reader, Type objectType)
        {
            throw new NotImplementedException();
        }

        public override void WriteValue(HessianWriter writer, object value)
        {
            if (!(value is Type))
            {
                throw Exceptions.UnExpectedTypeException(value.GetType());
            }

            Type type = (Type)value;

            (int index, bool isNewItem) = Context.TypeRefs.AddItem(type);

            if (isNewItem)
            {
                Context.StringConverter.WriteValue(writer, type.FullName);
            }
            else
            {
                Context.IntConverter.WriteValue(writer, index);
            }
        }
    }
}
