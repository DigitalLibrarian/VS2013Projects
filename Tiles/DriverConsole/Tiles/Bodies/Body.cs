﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Bodies.Health;
using Tiles.Bodies.Health.Injuries;

namespace Tiles.Bodies
{
    public class Body : IBody
    {
        public bool IsGrasping { get { return Parts.Any(x => x.IsGrasping); } }
        public bool IsBeingGrasped { get { return Parts.Any(x => x.IsBeingGrasped); } }
        public bool IsWrestling { get {  return Parts.Any(x => x.IsWrestling);} }
        public IList<IBodyPart> Parts { get; private set; }

        public IHealthState Health { get; set; }

        public Body(IList<IBodyPart> parts)
            : this(parts, new HealthState())
        {

        }

        public Body(IList<IBodyPart> parts, IHealthState healthState)
        {
            Parts = parts;
            Health = healthState;
        }

        public void Amputate(IBodyPart part)
        {
            foreach (var subPart in Parts.ToList())
            {
                if (subPart.Parent == part || subPart == part)
                {
                    Parts.Remove(subPart);
                }
            }
        }


        public void AddInjuries(IEnumerable<IInjury> injuries)
        {
            foreach (var injury in injuries)
            {
                Health.Add(injury);
            }
        }
    }
}
