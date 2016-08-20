using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DfNet.Raws
{
    public class DfObjectStore : IDfObjectStore
    {
        Dictionary<string, Dictionary<string, DfObject>> Db { get; set; }

        public DfObjectStore() : this(new DfObject[0]){}
        public DfObjectStore(IEnumerable<DfObject> objects) 
        {
            Db = new Dictionary<string, Dictionary<string, DfObject>>();
            foreach (var o in objects)
            {
                Add(o);
            }
        }

        public DfObject Get(string type, string name)
        {
            if (Db.ContainsKey(type))
            {
                if(Db[type].ContainsKey(name)){
                    return Db[type][name];
                }
            }
            return null;
        }

        public IEnumerable<DfObject> Get(string type)
        {
            if (Db.ContainsKey(type))
            {
                return Db[type].Values;
            }
            return Enumerable.Empty<DfObject>();
        }

        public void Add(DfObject o)
        {
            if (!Db.ContainsKey(o.Type))
            {
                Db[o.Type] = new Dictionary<string, DfObject>();
            }
            else if(Db[o.Type].ContainsKey(o.Name))
            {
                throw new DuplicateDfObjectNameException();
            }
            Db[o.Type][o.Name] = o;
        }
    }

    public class DuplicateDfObjectNameException : Exception { }
}
