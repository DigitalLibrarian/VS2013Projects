using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Bodies;

namespace Tiles.Agents
{
    public class AgentClass : IAgentClass
    {
        public string Name { get; set; }
        public Sprite Sprite { get; set; }
        public IBodyClass BodyClass { get; set; }

        public AgentClass(string name, Sprite sprite, IBodyClass bodyClass)
        {
            Name = name;
            Sprite = sprite;
            BodyClass = bodyClass;
        }
    }
}
