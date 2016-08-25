using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Content.Models
{
    public class Agent
    {
        public Agent(string name, Body body)
        {
            Name = name;
            Body = body;
        }
        public string Name { get; set; }
        public Body Body { get; set; }
    }
}
