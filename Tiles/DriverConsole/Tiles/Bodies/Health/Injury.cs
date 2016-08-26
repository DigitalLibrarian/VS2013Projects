using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Bodies.Health
{
    public class Injury : IInjury
    {
        public IInjuryClass Class { get; set; }
        public IBodyPart BodyPart { get; set; }
        public ITissueLayer TissueLayer { get; set; }

        public Injury(IInjuryClass injuryClass, IBodyPart bodyPart, ITissueLayer layer)
        {
            Class = injuryClass;
            BodyPart = bodyPart;
            TissueLayer = layer;

            if (injuryClass.UsesTtl)
            {
                Ttl = injuryClass.Ttl;
            }
        }

        public string Adjective { get { return Class.Adjective; } }
        public bool IsInstantDeath { get { return Class.IsInstantDeath; } }
        public bool IsPermanant { get { return Class.IsPermanant; } }
        public bool CanBeHealed { get { return Class.CanBeHealed; } }
        public bool CripplesBodyPart { get { return Class.CripplesBodyPart; } }

        public bool IsOver { get; private set; }
        public int Ttl { get; private set; }

        public virtual void Update(int ticks)
        {
            if (Class.UsesTtl)
            {
                Ttl = System.Math.Max(0, Ttl - ticks);
                IsOver = Ttl == 0;
            }
        }

        StringBuilder SB = new StringBuilder();
        public virtual string GetDisplayLabel()
        {
            SB.Clear();
            SB.Append(Adjective);
            if (BodyPart != null)
            {
                SB.AppendFormat(" {0} ", BodyPart.Name);
            }
            
            if (TissueLayer != null)
            {
                SB.AppendFormat(" {0} ", TissueLayer.Material.Adjective);
            }

            if (Class.UsesTtl)
            {
                SB.AppendFormat(" {0}/{1} ", Ttl, Class.Ttl);
            }

            return SB.ToString();
        }
    }
}
