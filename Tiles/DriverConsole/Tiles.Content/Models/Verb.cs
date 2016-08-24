using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Content.Models
{
    public class Verb
    {
        public bool IsTransitive { get; set; }
        public string SecondPerson { get; set; }
        public string ThirdPerson { get; set; }
    }
}
