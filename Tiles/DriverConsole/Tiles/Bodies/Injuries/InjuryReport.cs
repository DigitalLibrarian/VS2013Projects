using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents.Combat;
using Tiles.Bodies;
using Tiles.Materials;

namespace Tiles.Bodies.Injuries
{
    public interface IInjuryReport
    {
        IEnumerable<IBodyPartInjury> BodyPartInjuries { get; }

        IEnumerable<IBodyPartInjury> GetSeverings();

        bool IsSever(IBodyPart bodyPart);
        bool IsPrimaryTargetSevered();

        bool IsJam(out IBodyPart bp1, out ITissueLayer tl1, out IBodyPart bp2, out ITissueLayer tl2);
    }

    public class InjuryReport : IInjuryReport
    {
        public IEnumerable<IBodyPartInjury> BodyPartInjuries { get; set; }
        public InjuryReport(IEnumerable<IBodyPartInjury> injuries)
        {
            BodyPartInjuries = injuries;
        }

        public IEnumerable<IBodyPartInjury> GetSeverings()
        {
            return BodyPartInjuries.Where(x => x.IsSever);
        }

        public bool IsSever(IBodyPart bodyPart)
        {
            return GetSeverings().FirstOrDefault(x => x.BodyPart == bodyPart) != null;
        }

        public bool IsPrimaryTargetSevered()
        {
            return IsSever(BodyPartInjuries.First().BodyPart);
        }


        public bool IsJam(out IBodyPart bp1, out ITissueLayer tl1, out IBodyPart bp2, out ITissueLayer tl2)
        {
            bp1 = bp2 = null;
            tl1 = tl2 = null;

            for (int bpi = 0; bpi < BodyPartInjuries.Count(); bpi++)
            {
                var bpInjury = BodyPartInjuries.ElementAt(bpi);
                for (int tli = 0; tli < bpInjury.TissueLayerInjuries.Count(); tli++)
                {
                    var tlInjury = bpInjury.TissueLayerInjuries.ElementAt(tli);

                    if (bp1 == null)
                    {
                        if (tlInjury.IsBluntCrack)
                        {
                            bp1 = bpInjury.BodyPart;
                            tl1 = tlInjury.Layer;
                        }
                    }
                    else if (bp2 == null)
                    {
                        bp2 = bpInjury.BodyPart;
                        tl2 = tlInjury.Layer;
                    }
                    else break;
                }
            }

            return (bp1 != null && tl1 != null && bp2 != null && tl2 != null);
        }
    }
}
