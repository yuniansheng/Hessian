using System;
using System.Collections.Generic;
using System.Text;

namespace Hessian.IO.Converters
{
    public class ClassDefinitionConverter : HessianConverter
    {
        public override bool CanRead(byte initialOctet)
        {
            return Constants.BC_OBJECT_DEF == initialOctet;
        }

        public override object ReadValue(HessianReader reader, HessianContext context, Type objectType, byte initialOctet)
        {
            if (Constants.BC_OBJECT_DEF != initialOctet)
            {
                throw Exceptions.UnExpectedInitialOctet(this, initialOctet);
            }

            var type = (Type)TypeConverter.ReadValue(reader, context, null);
            var len = (int)IntConverter.ReadValue(reader, context, typeof(int));
            var fieldNames = new List<string>(len);
            for (int i = 0; i < len; i++)
            {
                var fieldName = (string)StringConverter.ReadValue(reader, context, typeof(string));
                fieldNames.Add(fieldName);
            }
            var definition = ClassDefinition.FromHessianDefinition(type, fieldNames);
            context.ClassRefs.AddItem(definition);
            return definition;
        }

        public override void WriteValue(HessianWriter writer, HessianContext context, object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (!(value is ClassDefinition))
            {
                throw new InvalidOperationException("value is not an class definition");
            }

            WriteClassDefinition(writer, context, value as ClassDefinition, out int definitionIndex);
        }

        public override void WriteValueNotNull(HessianWriter writer, HessianContext context, object value)
        {
            throw new NotImplementedException();
        }

        public void WriteClassDefinition(HessianWriter writer, HessianContext context, ClassDefinition definition, out int definitionIndex)
        {
            if (definition == null)
            {
                throw new ArgumentNullException(nameof(definition));
            }

            (var index, var isNewItem) = context.ClassRefs.AddItem(definition);
            if (isNewItem)
            {
                writer.Write(Constants.BC_OBJECT_DEF);
                TypeConverter.WriteType(writer, context, definition.Type);
                IntConverter.WriteInt(writer, context, definition.FieldNames.Count);
                foreach (var fieldName in definition.FieldNames)
                {
                    StringConverter.WriteValueNotNull(writer, context, fieldName);
                }
            }
            definitionIndex = index;
        }
    }
}
