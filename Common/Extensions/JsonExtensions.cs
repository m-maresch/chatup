using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Common.Extensions
{
    public static class JsonExtensions
    {
        public static string ToJson(this object value)
        {
            if (value == null) return null;
            string json = JsonConvert.SerializeObject(value);
            return json;
        }

        public static T ToObject<T>(this string json)
        {
            if (json == null) return default(T);
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
