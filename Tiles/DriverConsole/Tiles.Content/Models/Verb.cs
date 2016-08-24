using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Content.Models
{
    public class Verb
    {
        bool IsTransitive { get; set; }
        string SecondPerson { get; set; }
        string ThirdPerson { get; set; }
    }
}
