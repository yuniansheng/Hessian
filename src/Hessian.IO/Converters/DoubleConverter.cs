using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Hessian.IO.Converters
{
    public class DoubleConverter : HessianConverter
    {
        public override object ReadValue(HessianReader reader, HessianContext context, Type objectType)
        {
            throw new NotImplementedException();
        }

        public override void WriteValue(HessianWriter writer, HessianContext context, object value)
        {
            var type = value.GetType();
            double v = 0;
            if (type == typeof(double))
            {
                v = (double)value;
            }
            else if (type == typeof(float))
            {
                v = Convert.ToDouble(value);
            }
            else
            {
                throw Exceptions.UnExpectedTypeException(type);
            }

            int intValue = (int)v;

            if (intValue == v)
            {
                if (intValue == 0)
                {
                    writer.Write(Constants.BC_DOUBLE_ZERO);
                    return;
                }
                else if (intValue == 1)
                {
                    writer.Write(Constants.BC_DOUBLE_ONE);
                    return;
                }
                else if (-0x80 <= intValue && intValue < 0x80)
                {
                    writer.Write(Constants.BC_DOUBLE_BYTE);
                    writer.Write((byte)intValue);
                    return;
                }
                else if (-0x8000 <= intValue && intValue < 0x8000)
                {
                    writer.Write(Constants.BC_DOUBLE_SHORT);
                    writer.Write((short)intValue);
                    return;
                }
            }

            int mills = (int)(v * 1000);

            if (0.001 * mills == v)
            {
                writer.Write(Constants.BC_DOUBLE_MILL);
                writer.Write(mills);
                return;
            }

            writer.Write(Constants.BC_DOUBLE);
            writer.Write(v);
        }
    }
}
