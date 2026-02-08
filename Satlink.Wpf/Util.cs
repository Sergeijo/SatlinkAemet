using System.Collections.Generic;
using System.Reflection;

namespace Satlink
{
    public static class Util
    {
        public static List<string> GetMyProperties(this object obj)
        {
            var properties = new List<string>();

            foreach (PropertyInfo pinfo in obj.GetType().GetProperties())
            {
                properties.Add(pinfo.Name);
            }

            return properties;
        }
    }
}
