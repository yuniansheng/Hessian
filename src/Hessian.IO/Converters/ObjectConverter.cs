using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Hessian.IO.Converters
{
    public class ObjectConverter : ValueRefConverterBase
    {
        public override bool CanRead(byte initialOctet)
        {
            return ClassDefinitionConverter.CanRead(initialOctet) ||
                (0x60 <= initialOctet && initialOctet <= 0x6f) ||
                Constants.BC_OBJECT == initialOctet ||
                base.CanRead(initialOctet);
        }

        public override object ReadValueNotExisted(HessianReader reader, HessianContext context, Type objectType, byte initialOctet)
        {
            if (ClassDefinitionConverter.CanRead(initialOctet))
            {
                ClassDefinitionConverter.ReadValue(reader, context, typeof(ClassDefinition), initialOctet);
                initialOctet = reader.ReadByte();
            }

            int definitionIndex = 0;
            if (0x60 <= initialOctet && initialOctet <= 0x6f)
            {
                definitionIndex = initialOctet - Constants.BC_OBJECT_DIRECT;
            }
            else if (Constants.BC_OBJECT == initialOctet)
            {
                definitionIndex = (int)IntConverter.ReadValue(reader, context, typeof(int));
            }
            var definition = context.ClassRefs.GetItem(definitionIndex);

            object value = null;
            if (definition.Type.IsEnum)
            {
                definition.VisitFields(field =>
                {
                    var fieldValue = AutoConverter.ReadValue(reader, context, field.Type);
                    if (field.Name == "name")
                    {
                        value = Enum.Parse(definition.Type, (string)fieldValue);
                        context.ValueRefs.AddItem(value);
                    }
                });
            }
            else
            {
                value = Activator.CreateInstance(definition.Type);
                context.ValueRefs.AddItem(value);
                definition.VisitFields(field =>
                {
                    var fieldValue = AutoConverter.ReadValue(reader, context, field.Type);
                    field.Setter(value, fieldValue);
                });
            }
            return value;
        }

        public override void WriteValueNotExisted(HessianWriter writer, HessianContext context, object value)
        {
            var definition = ClassDefinition.ForType(value.GetType());
            ClassDefinitionConverter.WriteClassDefinition(writer, context, definition, out int definitionIndex);
            if (definitionIndex <= Constants.OBJECT_DIRECT_MAX)
            {
                writer.Write((byte)(Constants.BC_OBJECT_DIRECT + definitionIndex));
            }
            else
            {
                writer.Write(Constants.BC_OBJECT);
                writer.Write(definitionIndex);
            }
            definition.VisitFields(field =>
            {
                var fieldValue = field.Getter(value);
                AutoConverter.WriteValue(writer, context, fieldValue);
            });
        }
    }
}
