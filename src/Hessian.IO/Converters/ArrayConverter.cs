using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Hessian.IO.Converters
{
    public class ArrayConverter : ValueRefConverterBase
    {
        public override object ReadValue(HessianReader reader, HessianContext context, Type objectType)
        {
            throw new NotImplementedException();
        }

        public override void WriteValueNotExisted(HessianWriter writer, HessianContext context, object value)
        {
            Type type = value.GetType();
            if (!type.IsArray)
            {
                throw Exceptions.UnExpectedTypeException(type);
            }

            var elementType = type.GetElementType();
            if (elementType == typeof(byte) || elementType == typeof(sbyte))
            {
                BinaryConverter.WriteValueNotNull(writer, context, value);
                return;
            }

            var array = (Array)value;
            HessianConverter itemConverter = null;
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
                    IntConverter.WriteInt(writer, context, array.Length);
                }
                itemConverter = AutoConverter;
            }
            else
            {
                if (array.Length <= Constants.LIST_DIRECT_MAX)
                {
                    writer.Write((byte)(Constants.BC_LIST_DIRECT + array.Length));
                    TypeConverter.WriteValueNotNull(writer, context, type);
                }
                else
                {
                    writer.Write(Constants.BC_LIST_FIXED);
                    TypeConverter.WriteValueNotNull(writer, context, type);
                    IntConverter.WriteInt(writer, context, array.Length);
                }
                itemConverter = AutoConverter.GetConverter(elementType);
            }

            foreach (var item in array)
            {
                itemConverter.WriteValue(writer, context, item);
            }
        }
    }
}
