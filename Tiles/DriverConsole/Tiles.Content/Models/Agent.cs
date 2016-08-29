using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Content.Models
{
    public class Agent
    {
        public Agent(string name, Body body, Sprite sprite)
        {
            Name = name;
            Body = body;
            Sprite = sprite;
        }
        public string Name { get; set; }
        public Sprite Sprite { get; set; }
        public Body Body { get; set; }
    }
}
