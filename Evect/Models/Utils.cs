using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Evect.Models
{
    public class Utils
    {
        public static IEnumerable<T> GetEnumerableOfType<T>(params object[] constuctorArgs) where T : class
        {
            List<T> objects = new List<T>();
            foreach (Type type in Assembly.GetAssembly(typeof(T)).GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(T))))
            {
                objects.Add((T)Activator.CreateInstance(type, constuctorArgs));
            }

            return objects;
        }
    }
}