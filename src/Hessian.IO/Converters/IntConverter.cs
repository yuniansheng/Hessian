using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Hessian.IO.Converters
{
    public class IntConverter : HessianConverter
    {
        public override object ReadValue(HessianReader reader, Type objectType)
        {
            throw new NotImplementedException();
        }

        public override void WriteValue(HessianWriter writer, object value)
        {
            int v = (int)value;

            if (Constants.INT_DIRECT_MIN <= v && v <= Constants.INT_DIRECT_MAX)
                writer.Write((byte)(Constants.BC_INT_ZERO + v));
            else if (Constants.INT_BYTE_MIN <= v && v <= Constants.INT_BYTE_MAX)
            {
                writer.Write((byte)(Constants.BC_INT_BYTE_ZERO + (v >> 8)));
                writer.Write((byte)v);
            }
            else if (Constants.INT_SHORT_MIN <= v && v <= Constants.INT_SHORT_MAX)
            {
                writer.Write((byte)(Constants.BC_INT_SHORT_ZERO + (v >> 16)));
                writer.Write((short)v);
            }
            else
            {
                writer.Write(Constants.BC_INT);
                writer.Write(v);
            }
        }
    }
}
