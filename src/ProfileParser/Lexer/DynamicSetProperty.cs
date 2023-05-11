using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ProfileParser.Lexer
{
    internal class DynamicProperty
    {
        private static PropertyInfo GetProperty(Object obj, string[] value)
        {
            var propertyinfo = obj.GetType().GetProperties().Where(x => x.Name.ToLower() == value[0].Replace("_", "").Replace("-", "")).FirstOrDefault();
            if (propertyinfo is null)
            {
#if RELEASE
                // throw an exception just for undo set property name
                throw new ParserException("set parse exception");
#endif
            }
            return propertyinfo;
        }

        public static void UnSet(Object obj, string[] value)
        {
            var propertyinfo = GetProperty(obj, value);
            propertyinfo?.SetValue(obj, null);
        }

        public static void Set(Object obj, string[] value,object item = default)
        {
            var propertyinfo = GetProperty(obj,value);
            Console.WriteLine(propertyinfo?.Name + "\t" + propertyinfo?.PropertyType?.Name);

            switch (propertyinfo?.PropertyType?.Name)
            {
                case "Int32":
                    {
                        propertyinfo?.SetValue(obj, Convert.ToInt32(value[1].TrimEnd(';').Replace("\"", "")));
                    }
                    break;
                case "String":
                    {
                        propertyinfo?.SetValue(obj, String.Join(" ", value.Skip(1)).TrimEnd(';').Replace("\"", ""));
                    }
                    break;
                case "Boolean":
                    {
                        propertyinfo?.SetValue(obj, Convert.ToBoolean(value[1].TrimEnd(';').Replace("\"", "")));
                    }
                    break;
                default:
                    {
                        propertyinfo?.SetValue(obj, item);
                    }
                    break;
            }
        }
    }
}
