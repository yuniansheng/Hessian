using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Hessian.IO.Converters
{
    public class BoolConverter : HessianConverter
    {
        public override object ReadValue(HessianReader reader, Type objectType)
        {
            throw new NotImplementedException();
        }

        public override void WriteValue(HessianWriter writer, object value)
        {
            bool b = (bool)value;
            if (b)
            {
                writer.Write('T');
            }
            else
            {
                writer.Write('F');
            }
        }
    }
}
