using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Ecs;
using Tiles.Math;

namespace Tiles.EntityComponents
{
    public interface IParticleComponent : IComponent
    {
        Vector3 Position { get; set; }
    }

    public class ParticleComponent : IParticleComponent
    {
        public int Id { get { return ComponentTypes.Particle; } }
        public Vector3 Position { get; set; }
    }
}
