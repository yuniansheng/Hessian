using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Hessian.IO
{
    public class HessianSerializer
    {
        public HessianContext _context;

        public HessianSerializer()
        {
            _context = new HessianContext();
        }

        public void DeSerialize(Stream stream)
        {

        }

        public void Serialize(Stream stream, object value)
        {
            using (var writer = new HessianWriter(stream))
            {
                _context.AutoConverter.WriteValue(writer, value);
            }
        }
    }
}
