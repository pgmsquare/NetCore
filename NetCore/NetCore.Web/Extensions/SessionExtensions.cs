using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCore.Web.Extensions
{
    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, List<T> values)
        {
            session.SetString(key, JsonConvert.SerializeObject(values));
        }

        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            var sessionValue = session.GetString(key);

            //default 참조형식 null, 숫자형식 0
            return sessionValue != null ? JsonConvert.DeserializeObject<T>(sessionValue) : default(T);
        }
    }
}
