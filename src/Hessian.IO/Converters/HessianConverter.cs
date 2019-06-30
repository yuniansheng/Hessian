using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Hessian.IO.Converters
{
    public abstract class HessianConverter
    {
        public abstract object ReadValue(HessianReader reader, Type objectType);

        public abstract void WriteValue(HessianWriter writer, object value);
    }
}
