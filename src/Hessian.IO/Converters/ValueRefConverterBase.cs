using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Hessian.IO.Converters
{
    public abstract class ValueRefConverterBase : HessianConverter
    {
        public override bool CanRead(byte initialOctet)
        {
            return Constants.BC_REF == initialOctet;
        }

        public override object ReadValue(HessianReader reader, HessianContext context, Type objectType, byte initialOctet)
        {
            if (Constants.BC_REF == initialOctet)
            {
                var index = (int)IntConverter.ReadValue(reader, context, typeof(int));
                return context.ValueRefs.GetItem(index);
            }
            else
            {
                return ReadValueNotExisted(reader, context, objectType, initialOctet);
            }
        }

        public abstract object ReadValueNotExisted(HessianReader reader, HessianContext context, Type objectType, byte initialOctet);

        public override void WriteValueNotNull(HessianWriter writer, HessianContext context, object value)
        {
            (var index, var isNew) = context.ValueRefs.AddItem(value);
            if (isNew)
            {
                WriteValueNotExisted(writer, context, value);
            }
            else
            {
                writer.Write(Constants.BC_REF);
                IntConverter.WriteInt(writer, context, index);
            }
        }

        public abstract void WriteValueNotExisted(HessianWriter writer, HessianContext context, object value);
    }
}
