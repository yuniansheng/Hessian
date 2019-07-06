using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Hessian.IO
{
    public class ClassDefinition
    {
        public Type Type { get; set; }

        public Dictionary<string, Func<object, object>> FieldAccessors { get; set; }

        public ClassDefinition(Type type)
        {
            Type = type;
            FieldAccessors = new Dictionary<string, Func<object, object>>();
        }

        public void Add(string name, Func<object, object> accessor)
        {
            FieldAccessors[name] = accessor;
        }

        public void AddProperty(PropertyInfo property)
        {
            FieldAccessors[property.Name] = property.GetValue;
        }

        public void AddProperties(IEnumerable<PropertyInfo> properties)
        {
            foreach (var property in properties)
            {
                AddProperty(property);
            }
        }
    }
}
