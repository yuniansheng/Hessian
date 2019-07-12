using System;
using System.Collections.Generic;
using System.Text;

namespace com.caucho.model
{
    public class Car
    {
        public string color { get; set; }

        public string model { get; set; }

        public Car() { }

        public Car(string color, string model)
        {
            this.color = color;
            this.model = model;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var car = obj as Car;
            if (null == car)
            {
                return false;
            }

            return color == car.color && model == car.model;
        }
    }
}
