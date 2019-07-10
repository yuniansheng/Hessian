using Hessian.IO.Converters;
using Hessian.IO.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Hessian.IO
{
    public static class Exceptions
    {
        internal static InvalidOperationException UnExpectedTypeException(Type type)
        {
            return new InvalidOperationException(string.Format(Resources.UnExpectedType, type.ToString()));
        }

        internal static InvalidDataException UnExpectedInitialOctet(HessianConverter converter, byte initialOctet)
        {
            return new InvalidDataException(string.Format("{0} get unexpected initial octet x{1:x2}", converter.GetType().Name, initialOctet));
        }

        internal static void ThrowIfUnExpectedDataLength(int expectedLength, int actualLength)
        {
            if (expectedLength != actualLength)
            {
                throw new InvalidDataException(string.Format("expected length is {0} but actual is {1}", expectedLength, actualLength));
            }
        }
    }

    internal static class Error
    {
        internal static Exception GetFileNotOpen()
        {
            return new ObjectDisposedException(null, "ObjectDisposed_FileClosed");
        }

        internal static Exception GetEndOfFile()
        {
            return new EndOfStreamException("IO_EOF_ReadBeyondEOF");
        }
    }
}
