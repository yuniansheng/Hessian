﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Hessian.IO.Converters
{
    public class ListConverter : HessianConverter
    {
        public override object ReadValue(HessianReader reader, HessianContext context, Type objectType)
        {
            throw new NotImplementedException();
        }

        public override void WriteValue(HessianWriter writer, HessianContext context, object value)
        {
            Type type = value.GetType();
            if (type == typeof(ArrayList))
            {
                writer.Write(Constants.BC_LIST_VARIABLE_UNTYPED);
            }
            else if (IsGenericList(type))
            {
                var itemType = type.GenericTypeArguments[0];
                if (itemType == typeof(object))
                {
                    writer.Write(Constants.BC_LIST_VARIABLE_UNTYPED);
                }
                else
                {
                    writer.Write(Constants.BC_LIST_VARIABLE);
                    TypeConverter.WriteValue(writer, context, itemType);
                }
            }
            else
            {
                throw Exceptions.UnExpectedTypeException(type);
            }

            foreach (var item in (IEnumerable)value)
            {
                StringConverter.WriteValue(writer, context, item);
            }
            writer.Write(Constants.BC_END);
        }

        public bool IsList(Type type)
        {
            return type == typeof(ArrayList) || IsGenericList(type);
        }

        private bool IsGenericList(Type type)
        {
            return type.IsGenericType && typeof(List<>).IsAssignableFrom(type.GetGenericTypeDefinition());
        }
    }
}
