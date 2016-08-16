using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwarfFortressNet.RawModels
{
    public class ObjectDb
    {
        Dictionary<Type, Dictionary<string, object>> DB { get; set; }
        public ObjectDb()
        {
            DB = new Dictionary<Type, Dictionary<string, object>>();
        }

        public T Get<T>(string referenceName)
        {
            var type = typeof(T);
            if (DB.ContainsKey(type))
            {
                if (DB[type].ContainsKey(referenceName))
                {
                    return (T)DB[type][referenceName];
                }
            }
            return default(T);
        }

        public void Add<T>(string referenceName, T t)
        {
            var type = typeof(T);
            if (!DB.ContainsKey(type))
            {
                DB[type] = new Dictionary<string, object>();
            }
            DB[type][referenceName] = t;
        }
    }
}
