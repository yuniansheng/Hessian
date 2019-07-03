using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Hessian.IO.Converters
{
    public class ObjectConverter : HessianConverter
    {
        private Dictionary<Type, List<PropertyInfo>> _propertiesCache = new Dictionary<Type, List<PropertyInfo>>();

        public override object ReadValue(HessianReader reader, Type objectType)
        {
            throw new NotImplementedException();
        }

        public override void WriteValue(HessianWriter writer, object value)
        {
            Type t = value.GetType();
            (var index, var isNewItem) = Context.ClassRefs.AddItem(t);

            var properties = LoadProperties(t);
            if (isNewItem)
            {
                WriteClassDefinition(writer, t, properties);
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

            foreach (var property in properties)
            {
                object propertyValue = property.GetValue(value);
                Context.StringConverter.WriteValue(writer, propertyValue);
            }
        }

        private void WriteClassDefinition(HessianWriter writer, Type type, List<PropertyInfo> properties)
        {
            writer.Write(Constants.BC_OBJECT_DEF);
            Context.StringConverter.WriteValue(writer, type.FullName);

            Context.IntConverter.WriteValue(writer, properties.Count);

            foreach (var property in properties)
            {
                Context.StringConverter.WriteValue(writer, property.Name);
            }
        }

        private List<PropertyInfo> LoadProperties(Type type)
        {
            List<PropertyInfo> list;
            if (!_propertiesCache.TryGetValue(type, out list))
            {
                list = new List<PropertyInfo>();
                for (Type t = type; t != null; t = t.BaseType)
                {
                    PropertyInfo[] properties = t.GetProperties(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.GetProperty);
                    list.AddRange(properties);
                }
                _propertiesCache.Add(type, list);
            }

            return list;
        }
    }
}
