using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Math;

namespace Tiles.Bodies.Health
{
    public interface IWound
    {
        string Adjective { get; }
        WoundSeverity Severity { get; }
    }

    public enum WoundSeverity
    {
        None,
        LightlyWounded,
        ModeratelyWounded,
        Pulped
    }

    public interface IBodyPartWoundClassifier
    {
        WoundSeverity ClassifySeverity(IDamageVector damageVector);
    }

    public class BodyPartWoundClassifier : IBodyPartWoundClassifier
    {
        public WoundSeverity ClassifyFraction(Fraction damageFraction)
        {
            var damageRatio = damageFraction.AsDouble();

            if (damageRatio < 0.1)
            {
                return WoundSeverity.None;
            }
            else if (damageRatio < 0.25)
            {
                return WoundSeverity.LightlyWounded;
            }
            else
            {
                return WoundSeverity.ModeratelyWounded;
            }
        }

        public WoundSeverity ClassifySeverity(IDamageVector damageVector)
        {
            if (IsPulped(damageVector))
            {
                return WoundSeverity.Pulped;
            }

            long totalNum=0, totalDenom=0;
            foreach (var damageType in damageVector.GetComponentTypes())
            {
                var fraction = damageVector.GetFraction(damageType);
                totalNum += fraction.Numerator;
                totalDenom += fraction.Denominator;
            }

            return ClassifyFraction(new Fraction(totalNum, totalDenom));
        }

        bool IsPulped(IDamageVector damage)
        {
            foreach (var damageType in damage.GetComponentTypes())
            {
                double threshold = 1.0d;
                if (damageType == DamageType.Blunt)
                {
                    threshold = 2.5d;
                }

                var fraction = damage.GetFraction(damageType);
                if (fraction.AsDouble() > threshold)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
