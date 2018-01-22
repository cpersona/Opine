using System;
using System.Collections.Generic;
using System.Linq;

namespace Opine
{
    public static class Types
    {
        private static Dictionary<Type, string[]> names = 
            new Dictionary<Type, string[]>();

        public static string GetTypeNameString(Type type)
        {
            var parts = GetTypeName(type);
            return parts[0] + "." + parts[1];
        }

        public static string GetAggregateType(Type type)
        {
            return GetTypeName(type)[0];
        }

        private static string[] GetTypeName(Type type)
        {
            string[] parts = null;
            if (!names.TryGetValue(type, out parts))
            {
                var name = type.FullName;
                var temp = name.Split('.');
                var length = temp.Length;
                parts = new [] { temp[length - 2], temp[length - 1] };
                names.Add(type, parts);
            }
            return parts;
        }
    }
}