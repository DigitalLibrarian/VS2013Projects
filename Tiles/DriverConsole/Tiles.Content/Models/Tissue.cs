using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Content.Models
{
    public class Tissue
    {
        public Tissue()
        {
            Layers = new List<TissueLayer>();
        }
        public List<TissueLayer> Layers { get; set; }
    }
}
