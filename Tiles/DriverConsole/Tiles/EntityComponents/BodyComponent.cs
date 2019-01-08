using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Bodies;
using Tiles.Ecs;

namespace Tiles.EntityComponents
{
    public class BodyComponent : IComponent
    {
        public int Id { get { return ComponentTypes.Body; } }
        public IBody Body { get; set; }

        public BodyComponent(IBody body)
        {
            Body = body;
        }
    }
}
