using System;
using System.Collections.Generic;
using System.Text;

namespace com.caucho.model
{
    public class GenericType
    {
        public int code { get; set; }
        public object data { get; set; }

        public GenericType() { }

        public GenericType(int code, object data)
        {
            this.code = code;
            this.data = data;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var that = obj as GenericType;
            if (null == that)
            {
                return false;
            }

            return code == that.code && object.Equals(data, that.data);
        }
    }
}
