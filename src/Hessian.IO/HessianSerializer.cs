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

        public bool AutoReset { get; set; }

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
                if (AutoReset)
                {
                    Reset();
                }
            }
        }

        public byte[] Serialize(object value)
        {
            using (var stream = new MemoryStream())
            {
                Serialize(stream, value);
                return stream.ToArray();
            }
        }

        public void Reset()
        {
            _context.Reset();
        }
    }
}
