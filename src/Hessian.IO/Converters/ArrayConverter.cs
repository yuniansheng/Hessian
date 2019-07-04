using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Hessian.IO.Converters
{
    public class ArrayConverter : HessianConverter
    {
        public override object ReadValue(HessianReader reader, HessianContext context, Type objectType)
        {
            throw new NotImplementedException();
        }

        public override void WriteValue(HessianWriter writer, HessianContext context, object value)
        {
            Type type = value.GetType();
            if (!type.IsArray)
            {
                throw Exceptions.UnExpectedTypeException(type);
            }

            var elementType = type.GetElementType();
            if (elementType == typeof(byte) || elementType == typeof(sbyte))
            {
                BinaryConverter.WriteValue(writer, context, value);
                return;
            }

            var array = (Array)value;
            if (elementType == typeof(object))
            {
                //untyped array
                if (array.Length <= Constants.LIST_DIRECT_MAX)
                {
                    writer.Write((byte)(Constants.BC_LIST_DIRECT_UNTYPED + array.Length));
                }
                else
                {
                    writer.Write(Constants.BC_LIST_FIXED_UNTYPED);
                    IntConverter.WriteValue(writer, context, array.Length);
                }
            }
            else
            {
                if (array.Length <= Constants.LIST_DIRECT_MAX)
                {
                    writer.Write((byte)(Constants.BC_LIST_DIRECT + array.Length));
                    TypeConverter.WriteValue(writer, context, elementType);
                }
                else
                {
                    writer.Write(Constants.BC_LIST_FIXED);
                    TypeConverter.WriteValue(writer, context, elementType);
                    IntConverter.WriteValue(writer, context, array.Length);
                }
            }

            foreach (var item in array)
            {
                StringConverter.WriteValue(writer, context, item);
            }
        }
    }
}
