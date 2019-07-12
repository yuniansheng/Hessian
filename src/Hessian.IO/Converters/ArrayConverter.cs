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
        public override bool CanRead(byte initialOctet)
        {
            return (0x70 <= initialOctet && initialOctet <= 0x7f) ||
                (Constants.BC_LIST_FIXED_UNTYPED == initialOctet || Constants.BC_LIST_FIXED == initialOctet) ||
                base.CanRead(initialOctet);
        }

        public override object ReadValueNotExisted(HessianReader reader, HessianContext context, Type objectType, byte initialOctet)
        {
            int len = 0;
            Type elementType = null;
            if (0x70 <= initialOctet && initialOctet <= 0x77)
            {
                elementType = ((Type)TypeConverter.ReadValue(reader, context, null)).GetElementType();
                len = initialOctet - 0x70;
            }
            else if (0x78 <= initialOctet && initialOctet <= 0x7f)
            {
                elementType = typeof(object);
                len = initialOctet - 0x78;
            }
            else if (Constants.BC_LIST_FIXED_UNTYPED == initialOctet)
            {
                elementType = typeof(object);
                len = (int)IntConverter.ReadValue(reader, context, typeof(int));
            }
            else if (Constants.BC_LIST_FIXED == initialOctet)
            {
                elementType = ((Type)TypeConverter.ReadValue(reader, context, null)).GetElementType();
                len = (int)IntConverter.ReadValue(reader, context, typeof(int));
            }

            var array = Array.CreateInstance(elementType, len);
            context.ValueRefs.AddItem(array);

            HessianConverter itemConverter = null;
            if (elementType == typeof(object))
            {
                itemConverter = AutoConverter;
            }
            else
            {
                itemConverter = AutoConverter.GetConverter(elementType);
            }

            for (int i = 0; i < len; i++)
            {
                var item = itemConverter.ReadValue(reader, context, elementType);
                array.SetValue(item, i);
            }
            return array;
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
                GetConverter<BinaryConverter>().WriteValueNotNull(writer, context, value);
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
                    TypeConverter.WriteType(writer, context, type);
                }
                else
                {
                    writer.Write(Constants.BC_LIST_FIXED);
                    TypeConverter.WriteType(writer, context, type);
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
