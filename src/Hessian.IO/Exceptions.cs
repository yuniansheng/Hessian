using Hessian.IO.Properties;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hessian.IO
{
    public static class Exceptions
    {
        internal static InvalidOperationException UnExpectedTypeException(Type type)
        {
            return new InvalidOperationException(string.Format(Resources.UnExpectedType, type.FullName));
        }
    }
}
