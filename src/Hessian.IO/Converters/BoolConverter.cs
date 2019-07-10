using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Hessian.IO.Converters
{
    public class BoolConverter : HessianConverter
    {
        public override bool CanRead(byte initialOctet)
        {
            return initialOctet == Constants.BC_TRUE || initialOctet == Constants.BC_FALSE;
        }

        public override object ReadValue(HessianReader reader, HessianContext context, Type objectType)
        {
            throw new NotImplementedException();
        }

        public override object ReadValue(HessianReader reader, HessianContext context, Type objectType, byte initialOctet)
        {
            if (initialOctet == Constants.BC_TRUE)
            {
                return true;
            }
            else if (initialOctet == Constants.BC_FALSE)
            {
                return false;
            }
            else
            {
                throw Exceptions.UnExpectedInitialOctet(this, initialOctet);
            }
        }

        public override void WriteValueNotNull(HessianWriter writer, HessianContext context, object value)
        {
            var type = value.GetType();
            if (type != typeof(bool))
            {
                throw Exceptions.UnExpectedTypeException(type);
            }

            bool b = (bool)value;
            if (b)
            {
                writer.Write(Constants.BC_TRUE);
            }
            else
            {
                writer.Write(Constants.BC_FALSE);
            }
        }
    }
}
