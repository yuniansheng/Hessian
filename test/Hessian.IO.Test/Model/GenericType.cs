using System;
using System.Collections.Generic;
using System.Text;

namespace com.caucho.model
{
    public class GenericType
    {
        public int code { get; set; }
        public object data { get; set; }

        public GenericType(int code, object data)
        {
            this.code = code;
            this.data = data;
        }
    }
}
