using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Hessian.IO.Converters
{
    public class AutoConverter : HessianConverter
    {
        public override object ReadValue(HessianReader reader, Type objectType)
        {
            throw new NotImplementedException();
        }

        public override void WriteValue(HessianWriter writer, object value)
        {
            if (value == null)
            {
                writer.Write(Constants.BC_NULL);
                return;
            }

            var t = value.GetType();
            if (t.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                
            }            
        }
    }
}
