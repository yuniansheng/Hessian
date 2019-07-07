using System;
using System.Collections.Generic;
using System.Text;

namespace java.lang
{
    public class Class
    {
        public string name { get; set; }

        public Class()
        {

        }

        public Class(string name)
        {
            this.name = name;
        }

        public static Class OfName(string name)
        {
            return new Class(name);
        }

        public static Class OfType(Type type)
        {
            return new Class(type.ToString());
        }

        public static Class OfType<T>()
        {
            return new Class(typeof(T).ToString());
        }
    }
}
