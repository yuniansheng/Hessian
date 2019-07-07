using java.lang;
using System;
using System.Collections.Generic;
using System.Text;

namespace com.caucho.model
{
    public class XxlJob
    {
        public Class[] requestParameterTypes { get; set; }
        public object[] requestParameters { get; set; }

        public string responseMessage { get; set; }
        public object responseResult { get; set; }
    }
}
