using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Hessian.IO.Converters
{
    public class TypeConverter : HessianConverter
    {
        public override object ReadValue(HessianReader reader, HessianContext context, Type objectType)
        {
            throw new NotImplementedException();
        }

        public override void WriteValue(HessianWriter writer, HessianContext context, object value)
        {
            if (!(value is Type))
            {
                throw Exceptions.UnExpectedTypeException(value.GetType());
            }

            Type type = (Type)value;

            (int index, bool isNewItem) = context.TypeRefs.AddItem(type);

            if (isNewItem)
            {
                StringConverter.WriteValue(writer, context, type.FullName);
            }
            else
            {
                IntConverter.WriteValue(writer, context, index);
            }
        }
    }
}
