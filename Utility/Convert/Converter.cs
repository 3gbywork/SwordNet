using System;
using System.Reflection;

namespace Utility.Convert
{
    public class Converter
    {
        public static T TryParse<T>(object value)
        {
            return TryParse(value, default(T));
        }

        public static T TryParse<T>(object value, T defaultValue)
        {
            if (value == null)
            {
                return defaultValue;
            }
            Type type = typeof(T);
            // 泛型Nullable判断
            if (type.IsGenericType)
            {
                type = type.GetGenericArguments()[0];
            }
            // string类型没有TryParse方法，直接返回value
            if (type.Name.ToLower() == "string")
            {
                if (string.IsNullOrEmpty(value.ToString()))
                {
                    return defaultValue;
                }
                return (T)value;
            }

            var TryParse = type.GetMethod("TryParse", BindingFlags.Static | BindingFlags.Public,
                Type.DefaultBinder, new Type[] { typeof(string), type.MakeByRefType() },
                new ParameterModifier[] { new ParameterModifier(2) });
            var parameters = new object[] { value, Activator.CreateInstance(type) };
            if ((bool)TryParse.Invoke(null, parameters))
            {
                return (T)parameters[1];
            }

            return defaultValue;
        }
    }
}
