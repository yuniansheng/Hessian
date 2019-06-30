using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Hessian.IO.Converters
{
    public class LongConverter : HessianConverter
    {
        public override object ReadValue(HessianReader reader, Type objectType)
        {
            throw new NotImplementedException();
        }

        public override void WriteValue(HessianWriter writer, object value)
        {
            long v = (long)value;

            if (Constants.LONG_DIRECT_MIN <= v && v <= Constants.LONG_DIRECT_MAX)
            {
                writer.Write((byte)(Constants.BC_LONG_ZERO + v));
            }
            else if (Constants.LONG_BYTE_MIN <= v && v <= Constants.LONG_BYTE_MAX)
            {
                writer.Write((byte)(Constants.BC_LONG_BYTE_ZERO + (v >> 8)));
                writer.Write((byte)v);
            }
            else if (Constants.LONG_SHORT_MIN <= v && v <= Constants.LONG_SHORT_MAX)
            {
                writer.Write((byte)(Constants.BC_LONG_SHORT_ZERO + (v >> 16)));
                writer.Write((short)v);
            }
            else if (Constants.LONG_INT_MIN <= v && v <= Constants.LONG_INT_MAX)
            {
                writer.Write((byte)(Constants.BC_LONG_INT));
                writer.Write((int)v);
            }
            else
            {
                writer.Write(Constants.BC_LONG);
                writer.Write(v);
            }
        }
    }
}
