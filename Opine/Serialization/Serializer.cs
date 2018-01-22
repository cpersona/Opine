using System.Text;
using Newtonsoft.Json;

namespace Opine.Serialization
{
    public static class Serializer
    {
        private static readonly JsonSerializerSettings settings = 
            new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };
        
        public static string ToString(object value)
        {
            return JsonConvert.SerializeObject(value, settings);
        }

        public static string ToString(byte[] value)
        {
            return Encoding.UTF8.GetString(value);
        }

        public static object ToObject(string value)
        {
            return JsonConvert.DeserializeObject(value, settings);
        }

        public static object ToObject(byte[] value)
        {
            return ToObject(ToString(value));
        }

        public static byte[] ToByteArray(string value)
        {
            return Encoding.UTF8.GetBytes(value);
        }

        public static byte[] ToByteArray(object value)
        {
            return ToByteArray(ToString(value));
        }
    }
}