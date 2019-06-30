using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Hessian.IO.Converters
{
    public class StringConverter : HessianConverter
    {
        public override object ReadValue(HessianReader reader, Type objectType)
        {
            throw new NotImplementedException();
        }

        public override void WriteValue(HessianWriter writer, object value)
        {
            string s = (string)value;
            int length = s.Length;
            int index = 0;

            while (index + Constants.BINARY_CHUNK_SIZE < length)
            {
                writer.Write(Constants.BC_STRING_CHUNK);
                writer.Write((short)Constants.STRING_CHUNK_SIZE);
                writer.Write(s.Substring(index, Constants.STRING_CHUNK_SIZE));
                index += Constants.BINARY_CHUNK_SIZE;
            }

            int leftSize = length - index;
            if (leftSize == 0)
            {
                if (index == 0)
                {
                    writer.Write(Constants.BC_STRING_DIRECT);
                }
            }
            else
            {
                if (leftSize <= Constants.STRING_DIRECT_MAX)
                {
                    writer.Write((byte)(Constants.BC_STRING_DIRECT + leftSize));
                }
                else if (leftSize <= Constants.STRING_SHORT_MAX)
                {
                    writer.Write((byte)(Constants.BC_STRING_SHORT + (leftSize >> 8)));
                    writer.Write((byte)leftSize);
                }
                else
                {
                    writer.Write(Constants.BC_STRING);
                    writer.Write((short)leftSize);
                }

                writer.Write(s.Substring(index, leftSize));
            }
        }
    }
}
