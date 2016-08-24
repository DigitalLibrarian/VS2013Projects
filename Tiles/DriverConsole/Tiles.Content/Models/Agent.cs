using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Content.Models
{
    public class Agent
    {
        public Agent()
        {
            Body = new Body();
        }
        public Body Body { get; set; }
    }
}
