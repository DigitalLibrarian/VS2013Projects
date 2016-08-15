using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Bodies
{
    public class HealthVector
    {
        public const int MinHealth = 0;
        public const int MaxHealth = 100;
        private int _Health;
        public uint Health { get { return (uint)_Health; } }
        public virtual bool OutOfHealth { get { return Health == MinHealth; } }

        public HealthVector() : this(MaxHealth)
        {

        }

        public HealthVector(uint health)
        {
            _Health = (int)health;
            Clamp();
        }

        public static HealthVector Create()
        {
            return new HealthVector(MaxHealth);
        }

        public virtual void TakeDamage(uint dmg)
        {
            _Health -= (int)dmg;
            Clamp();
        }

        void Clamp()
        {
            _Health = System.Math.Max(MinHealth, _Health);
            _Health = System.Math.Min(MaxHealth, _Health);
        }

        public override string ToString()
        {
            return string.Format("{0}/{1}", Health, MaxHealth);
        }

        // TODO - add fuzzy classification property (enum with Low, Medium, High values)
    }
}
