using Hessian.IO.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hessian.IO
{
    public class HessianContext
    {
        public ReferenceMap<Type> TypeRefs { get; set; } = new ReferenceMap<Type>();
        public ReferenceMap<Type> ClassRefs { get; set; } = new ReferenceMap<Type>();
        public ReferenceMap<object> ValueRefs { get; set; } = new ReferenceMap<object>();

        public HessianContext()
        {
        }

        public void Reset()
        {
            TypeRefs.Clear();
            ClassRefs.Clear();
        }
    }
}
