using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Hessian.IO
{
    public class ClassDefinition
    {
        public Type Type { get; set; }

        public List<string> FieldNames { get; set; }

        public Dictionary<string, FieldDefinition> FieldDefinitions { get; set; }

        public ClassDefinition(Type type)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            FieldDefinitions = new Dictionary<string, FieldDefinition>();
            FieldNames = new List<string>();
        }

        public void Add(string name, Type fieldType, Func<object, object> getter, Action<object, object> setter)
        {
            FieldNames.Add(name);
            var filedDefinition = new FieldDefinition()
            {
                Name = name,
                Type = fieldType,
                Getter = getter,
                Setter = setter
            };
            FieldDefinitions[name] = filedDefinition;
        }

        public void AddProperties(IEnumerable<PropertyInfo> properties)
        {
            foreach (var property in properties)
            {
                Add(property.Name, property.PropertyType, property.GetValue, property.SetValue);
            }
        }

        public void VisitFields(Action<FieldDefinition> action)
        {
            foreach (var name in FieldNames)
            {
                var fieldDef = FieldDefinitions[name];
                action(fieldDef);
            }
        }

        public override int GetHashCode()
        {
            return Type.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var that = obj as ClassDefinition;
            if (null == that)
            {
                return false;
            }

            return Type == that.Type;
        }


        private static ConcurrentDictionary<Type, ClassDefinition> ClassDefinitionCache = new ConcurrentDictionary<Type, ClassDefinition>();

        public static ClassDefinition ForType(Type type)
        {
            return ClassDefinitionCache.GetOrAdd(type, t =>
            {
                var definition = new ClassDefinition(t);

                if (t.IsEnum)
                {
                    definition.Add("name", typeof(string), value => value.ToString(), null);
                }
                else
                {
                    for (; t != null; t = t.BaseType)
                    {
                        PropertyInfo[] properties = t.GetProperties(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.GetProperty);
                        definition.AddProperties(properties);
                    }
                }
                return definition;
            });
        }

        public static ClassDefinition FromHessianDefinition(Type type, List<string> fieldNames)
        {
            var definition = ForType(type);
            definition.FieldNames = fieldNames;
            return definition;
        }
    }

    public class FieldDefinition
    {
        public string Name { get; set; }

        public Type Type { get; set; }

        public Func<object, object> Getter { get; set; }

        public Action<object, object> Setter { get; set; }
    }
}
