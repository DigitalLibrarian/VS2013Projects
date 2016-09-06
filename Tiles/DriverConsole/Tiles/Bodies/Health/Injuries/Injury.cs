using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Bodies.Health.Injuries
{
    // TODO - this need some type of calculated magnitude to allow fuzzy gauging of severity 
    public class Injury : IInjury
    {
        public IInjuryClass Class { get; set; }
        public IBodyPart BodyPart { get; set; }
        public IDamageVector Damage { get; set; }
        
        public Injury(IInjuryClass injuryClass, IBodyPart bodyPart, IDamageVector damage)
        {
            Class = injuryClass;
            BodyPart = bodyPart;
            Damage = damage;

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

        public bool RemovesBodyPart { get { return Class.RemovesBodyPart; } }
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
            
            if (Class.UsesTtl)
            {
                SB.AppendFormat(" {0}/{1} ", Ttl, Class.Ttl);
            }

            return SB.ToString();
        }
    }
}
