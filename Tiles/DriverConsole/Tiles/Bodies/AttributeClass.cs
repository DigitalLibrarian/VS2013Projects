using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Bodies
{
    public interface IAttributeClass
    {
        string Name { get; }
        int Median { get; }
    }
    public class AttributeClass : IAttributeClass
    {
        public string Name { get; set; }
        public int Median { get; set; }
        public AttributeClass(string name, int median)
        {
            Name = name;
            Median = median;
        }
    }
}
