using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Hessian.IO.Converters
{
    public class MapConverter : HessianConverter
    {
        public override object ReadValue(HessianReader reader, Type objectType)
        {
            throw new NotImplementedException();
        }

        public override void WriteValue(HessianWriter writer, object value)
        {
            Type t = value.GetType();

            writer.Write(Constants.BC_MAP_UNTYPED);

            var kvType = typeof(KeyValuePair<,>).MakeGenericType(t.GetGenericArguments());
            var keyProperty = kvType.GetProperty("Key");
            var valueProperty = kvType.GetProperty("Value");
            foreach (var entry in (IEnumerable)value)
            {
                Context.IntConverter.WriteValue(writer, keyProperty.GetValue(entry));
                Context.StringConverter.WriteValue(writer, valueProperty.GetValue(entry));
            }

            writer.Write(Constants.BC_END);
        }
    }
}
