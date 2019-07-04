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
        /// <summary>
        /// if value existed in the ref then write the ref index
        /// </summary>
        /// <returns>true if value is existed in the ref, or return false</returns>
        public bool WriteRefIfValueExisted(HessianWriter writer, HessianContext context, object value)
        {
            (var index, var isNew) = context.ValueRefs.AddItem(value);
            if (!isNew)
            {
                writer.Write(Constants.BC_REF);
                IntConverter.WriteInt(writer, index);
            }

            return !isNew;
        }
    }
}
