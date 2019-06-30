using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Hessian.IO.Converters
{
    public class BinaryConverter : HessianConverter
    {
        public override object ReadValue(HessianReader reader, Type objectType)
        {
            throw new NotImplementedException();
        }

        public override void WriteValue(HessianWriter writer, object value)
        {
            byte[] bytes = (byte[])value;
            int length = bytes.Length;
            int index = 0;

            while (index + Constants.BINARY_CHUNK_SIZE < length)
            {
                writer.Write(Constants.BC_BINARY_CHUNK);
                writer.Write((short)Constants.BINARY_CHUNK_SIZE);
                writer.Write(bytes, index, Constants.BINARY_CHUNK_SIZE);
                index += Constants.BINARY_CHUNK_SIZE;
            }

            int leftSize = length - index;
            if (leftSize == 0)
            {
                if (index == 0)
                {
                    writer.Write(Constants.BC_BINARY_DIRECT);
                }
            }
            else
            {
                if (leftSize <= Constants.BINARY_DIRECT_MAX)
                {
                    writer.Write((byte)(Constants.BC_BINARY_DIRECT + leftSize));
                }
                else
                {
                    writer.Write(Constants.BC_BINARY);
                    writer.Write((short)leftSize);
                }

                writer.Write(bytes, index, leftSize);
            }
        }
    }
}
