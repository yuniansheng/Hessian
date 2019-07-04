using Hessian.IO.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Hessian.IO
{
    public class HessianSerializer
    {
        private HessianContext _context;

        public HessianSerializer()
        {
            _context = new HessianContext();
        }

        public void DeSerialize(Stream stream)
        {

        }

        public void Serialize(Stream stream, object value)
        {
            using (var writer = new HessianWriter(stream, new UTF8Encoding(false, true), true))
            {
                HessianConverter.AutoConverter.WriteValue(writer, _context, value);
            }
        }

        public void Reset()
        {
            _context.Reset();
        }
    }
}
