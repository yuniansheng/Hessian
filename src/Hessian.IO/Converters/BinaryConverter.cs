using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Hessian.IO.Converters
{
    public class BinaryConverter : HessianConverter
    {
        private byte[] _buffer;

        public override bool CanRead(byte initialOctet)
        {
            return initialOctet == Constants.BC_BINARY_CHUNK ||
                initialOctet == Constants.BC_BINARY ||
                (Constants.BC_BINARY_DIRECT <= initialOctet && initialOctet <= Constants.BC_BINARY_DIRECT + Constants.BINARY_DIRECT_MAX);
        }

        public override object ReadValue(HessianReader reader, HessianContext context, Type objectType)
        {
            throw new NotImplementedException();
        }

        public override object ReadValue(HessianReader reader, HessianContext context, Type objectType, byte initialOctet)
        {
            using (var stream = new MemoryStream())
            {
                while (initialOctet == Constants.BC_BINARY_CHUNK)
                {
                    int len = reader.ReadInt16();
                    FillBuffer(reader, stream, len);
                    initialOctet = reader.ReadByte();
                }

                if (initialOctet == Constants.BC_BINARY)
                {
                    int len = reader.ReadInt16();
                    FillBuffer(reader, stream, len);
                }
                else if (Constants.BC_BINARY_DIRECT <= initialOctet && initialOctet <= Constants.BC_BINARY_DIRECT + Constants.BINARY_DIRECT_MAX)
                {
                    FillBuffer(reader, stream, initialOctet - Constants.BC_BINARY_DIRECT);
                }
                else
                {
                    throw Exceptions.UnExpectedInitialOctet(this, initialOctet);
                }
                return stream.ToArray();
            }
        }

        private void FillBuffer(HessianReader reader, Stream destStream, int length)
        {
            if (_buffer == null || _buffer.Length < length)
            {
                _buffer = new byte[length];
            }
            var readLength = reader.Read(_buffer, 0, length);
            Exceptions.ThrowIfUnExpectedDataLength(length, readLength);
            destStream.Write(_buffer, 0, length);
        }

        public override void WriteValueNotNull(HessianWriter writer, HessianContext context, object value)
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
