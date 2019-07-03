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
        public override object ReadValue(HessianReader reader, Type objectType)
        {
            throw new NotImplementedException();
        }

        public override void WriteValue(HessianWriter writer, object value)
        {
            Type type = value.GetType();
            if (!type.IsArray)
            {
                throw Exceptions.UnExpectedTypeException(type);
            }

            var elementType = type.GetElementType();
            if (elementType == typeof(byte) || elementType == typeof(sbyte))
            {
                Context.BinaryConverter.WriteValue(writer, value);
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
                    Context.IntConverter.WriteValue(writer, array.Length);
                }
            }
            else
            {
                if (array.Length <= Constants.LIST_DIRECT_MAX)
                {
                    writer.Write((byte)(Constants.BC_LIST_DIRECT + array.Length));
                    Context.TypeConverter.WriteValue(writer, elementType);
                }
                else
                {
                    writer.Write(Constants.BC_LIST_FIXED);
                    Context.TypeConverter.WriteValue(writer, elementType);
                    Context.IntConverter.WriteValue(writer, array.Length);
                }
            }

            foreach (var item in array)
            {
                Context.StringConverter.WriteValue(writer, item);
            }
        }
    }
}
