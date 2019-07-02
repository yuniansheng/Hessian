using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Hessian.IO.Converters
{
    public class ListConverter : HessianConverter
    {
        public override object ReadValue(HessianReader reader, Type objectType)
        {
            throw new NotImplementedException();
        }

        public override void WriteValue(HessianWriter writer, object value)
        {
            Type t = value.GetType();
            if (t.IsArray)
            {
                WriteArray(writer, t, (Array)value);
            }
            else
            {
                WriteList(writer, t, value);
            }
        }

        private void WriteArray(HessianWriter writer, Type t, Array array)
        {
            if (t.GetElementType() == typeof(object))
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
                    Context.TypeConverter.WriteValue(writer, t.GetElementType());
                }
                else
                {
                    writer.Write(Constants.BC_LIST_FIXED);
                    Context.TypeConverter.WriteValue(writer, t.GetElementType());
                    Context.IntConverter.WriteValue(writer, array.Length);
                }
            }

            foreach (var item in array)
            {
                Context.StringConverter.WriteValue(writer, item);
            }
        }

        private void WriteList(HessianWriter writer, Type t, object value)
        {
            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(List<>) && t.GenericTypeArguments[0] != typeof(object))
            {
                writer.Write(Constants.BC_LIST_VARIABLE);
                var itemType = t.GenericTypeArguments[0];
                Context.TypeConverter.WriteValue(writer, itemType);
            }
            else
            {
                //ArrayList
                writer.Write(Constants.BC_LIST_VARIABLE_UNTYPED);
            }

            foreach (var item in (IEnumerable)value)
            {
                Context.StringConverter.WriteValue(writer, item);
            }

            writer.Write(Constants.BC_END);
        }
    }
}
