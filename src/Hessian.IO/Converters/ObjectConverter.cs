using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Hessian.IO.Converters
{
    public class ObjectConverter : ValueRefConverterBase
    {
        private Dictionary<Type, ClassDefinition> _classDefinitionCache = new Dictionary<Type, ClassDefinition>();

        public override object ReadValue(HessianReader reader, HessianContext context, Type objectType)
        {
            throw new NotImplementedException();
        }

        public override void WriteValueNotExisted(HessianWriter writer, HessianContext context, object value)
        {
            var definition = LoadClassDefinition(value.GetType());
            WriteClassDefinitionAndObjectBegin(writer, context, definition);
            WriteObjectBody(writer, context, value, definition);
        }

        private ClassDefinition WriteClassDefinitionAndObjectBegin(HessianWriter writer, HessianContext context, ClassDefinition definition)
        {
            var type = definition.Type;
            (var index, var isNewItem) = context.ClassRefs.AddItem(type);
            if (isNewItem)
            {
                writer.Write(Constants.BC_OBJECT_DEF);
                TypeConverter.WriteType(writer, context, type);

                IntConverter.WriteInt(writer, context, definition.FieldAccessors.Count);

                foreach (var property in definition.FieldAccessors)
                {
                    StringConverter.WriteValueNotNull(writer, context, property.Key);
                }
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
            return definition;
        }

        protected virtual void WriteObjectBody(HessianWriter writer, HessianContext context, object value, ClassDefinition definition)
        {
            foreach (var fieldAccessor in definition.FieldAccessors.Values)
            {
                object fieldValue = fieldAccessor(value);
                AutoConverter.WriteValue(writer, context, fieldValue);
            }
        }

        protected virtual ClassDefinition LoadClassDefinition(Type type)
        {
            ClassDefinition definition = null;
            if (!_classDefinitionCache.TryGetValue(type, out definition))
            {
                definition = new ClassDefinition(type);
                for (Type t = type; t != null; t = t.BaseType)
                {
                    PropertyInfo[] properties = t.GetProperties(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.GetProperty);
                    definition.AddProperties(properties);
                }
                _classDefinitionCache.Add(type, definition);
            }

            return definition;
        }
    }
}
