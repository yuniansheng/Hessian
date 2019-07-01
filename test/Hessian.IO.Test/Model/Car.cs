using System;
using System.Collections.Generic;
using System.Text;

namespace com.caucho.model
{
    public class Car
    {
        public string color { get; set; }

        public string model { get; set; }

        public Car(string color, string model)
        {
            this.color = color;
            this.model = model;
        }
    }
}
