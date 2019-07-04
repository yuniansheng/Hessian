using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Hessian.IO.Converters
{
    public class EnumConverter : ValueRefConverterBase
    {
        public override object ReadValue(HessianReader reader, HessianContext context, Type objectType)
        {
            throw new NotImplementedException();
        }

        public override void WriteValue(HessianWriter writer, HessianContext context, object value)
        {
            Type t = value.GetType();
            if (!t.IsEnum)
            {
                throw Exceptions.UnExpectedTypeException(t);
            }

            if (WriteRefIfValueExisted(writer, context, value))
            {
                return;
            }

            (var index, var isNewItem) = context.ClassRefs.AddItem(t);

            if (isNewItem)
            {
                WriteClassDefinition(writer, context, t);
            }

            if (index <= Constants.OBJECT_DIRECT_MAX)
            {
                writer.Write((byte)(Constants.BC_OBJECT_DIRECT + index));
            }
            else
            {
                writer.Write(Constants.BC_OBJECT);
                writer.Write(index);
            }

            StringConverter.WriteValue(writer, context, value.ToString());
        }

        private void WriteClassDefinition(HessianWriter writer, HessianContext context, Type type)
        {
            writer.Write(Constants.BC_OBJECT_DEF);
            StringConverter.WriteValue(writer, context, type.FullName);

            IntConverter.WriteValue(writer, context, 1);
            StringConverter.WriteValue(writer, context, "name");
        }
    }
}
