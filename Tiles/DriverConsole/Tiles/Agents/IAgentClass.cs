using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Bodies;

namespace Tiles.Agents
{
    public interface IAgentClass
    {
        string Name { get; set; }
        Sprite Sprite { get; set; }
        IBodyClass BodyClass { get; set; }

    }
}
