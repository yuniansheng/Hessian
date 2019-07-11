using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Hessian.IO.Converters
{
    public class StringConverter : HessianConverter
    {
        public override bool CanRead(byte initialOctet)
        {
            return (0x00 <= initialOctet && initialOctet <= 0x1f) ||
                 (0x30 <= initialOctet && initialOctet <= 0x34) ||
                 (Constants.BC_STRING_CHUNK == initialOctet || Constants.BC_STRING == initialOctet);
        }

        public override object ReadValue(HessianReader reader, HessianContext context, Type objectType)
        {
            throw new NotImplementedException();
        }

        public override object ReadValue(HessianReader reader, HessianContext context, Type objectType, byte initialOctet)
        {
            var builder = new StringBuilder();
            int len = 0;
            while (Constants.BC_STRING_CHUNK == initialOctet)
            {
                len = reader.ReadUInt16();
                builder.Append(reader.ReadString(len));
                initialOctet = reader.ReadByte();
            }

            if (0x00 <= initialOctet && initialOctet <= 0x1f)
            {
                len = initialOctet;
            }
            else if (0x30 <= initialOctet && initialOctet <= 0x34)
            {
                var b0 = reader.ReadByte();
                len = ((initialOctet - 0x30) << 8) + b0;
            }
            else if (Constants.BC_STRING == initialOctet)
            {
                len = reader.ReadUInt16();
            }
            else
            {
                throw Exceptions.UnExpectedInitialOctet(this, initialOctet);
            }

            builder.Append(reader.ReadString(len));
            return builder.ToString();
        }

        public override void WriteValueNotNull(HessianWriter writer, HessianContext context, object value)
        {
            var type = value.GetType();
            string s = null;
            if (type == typeof(string))
            {
                s = (string)value;
            }
            else if (type == typeof(char))
            {
                s = value.ToString();
            }
            else
            {
                throw Exceptions.UnExpectedTypeException(type);
            }

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
