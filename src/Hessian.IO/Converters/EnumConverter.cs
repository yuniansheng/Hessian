using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Hessian.IO.Converters
{
    public class EnumConverter : ObjectConverter
    {
        public override object ReadValue(HessianReader reader, HessianContext context, Type objectType)
        {
            throw new NotImplementedException();
        }

        public override void WriteValueNotExisted(HessianWriter writer, HessianContext context, object value)
        {
            Type t = value.GetType();
            if (!t.IsEnum)
            {
                throw Exceptions.UnExpectedTypeException(t);
            }

            base.WriteValueNotExisted(writer, context, value);
        }

        protected override ClassDefinition LoadClassDefinition(Type type)
        {
            var definition = new ClassDefinition(type);
            definition.Add("name", value => value.ToString());
            return definition;
        }
    }
}
