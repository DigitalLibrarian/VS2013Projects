using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents.Combat;
using Tiles.Materials;

namespace Tiles.Bodies.Health.Injuries
{
    public class InjuryResultBuilder : IInjuryResultBuilder
    {
        IDamageVector Damage { get; set; }
        IInjuryFactory InjuryFactory { get; set; }

        IBodyPart BodyPart { get; set; }
        List<IDamageResistor> ArmorResistors { get; set; }
        List<ITissueLayer> Layers { get; set; }
        Dictionary<ITissueLayer, IDamageResistor> LayerResistors { get; set; }
        Dictionary<ITissueLayer, IBodyPart> LayerParts { get; set; }

        public InjuryResultBuilder(IInjuryFactory injuryFactory, IDamageVector damage)
        {
            InjuryFactory = injuryFactory;
            Damage = damage;

            ArmorResistors = new List<IDamageResistor>();
            Layers = new List<ITissueLayer>();
            LayerResistors = new Dictionary<ITissueLayer, IDamageResistor>();
            LayerParts = new Dictionary<ITissueLayer, IBodyPart>();
        }
        
       
        IEnumerable<IInjury> CheckTissueDamage(ITissueLayer tl)
        {
            foreach (var damageType in Damage.GetComponentTypes())
            {
                switch (damageType)
                {
                    case DamageType.Blunt:
                        if (tl.CanBeBruised)
                        {
                            yield return InjuryFactory
                                .Create(
                                    StandardInjuryClasses.BruisedTissueLayer,
                                    BodyPart,
                                    tl);
                        }
                        break;
                    case DamageType.Pierce:
                        if (tl.CanBePunctured)
                        {
                            yield return InjuryFactory
                                .Create(
                                    StandardInjuryClasses.PuncturedTissueLayer,
                                    BodyPart,
                                    tl);
                        }
                        break;
                    case DamageType.Slash:
                        if (tl.CanBeTorn)
                        {
                            yield return InjuryFactory
                                .Create(
                                    StandardInjuryClasses.TornTissueLayer,
                                    BodyPart,
                                    tl);
                        }
                        break;
                }
            }
        }

        public void AddDamage(IDamageVector damage)
        {
            Damage.Add(damage);
        }

        public void AddArmorResistor(IDamageResistor resistor)
        {
            ArmorResistors.Add(resistor);
        }

        public void SetTargetBodyPart(IBodyPart bodyPart)
        {
            BodyPart = bodyPart;
        }

        public void AddTissueResistor(ITissueLayer layer, IDamageResistor resistor)
        {
            Layers.Add(layer);
            LayerResistors[layer] = resistor;
        }
        
        public IEnumerable<IInjury> Build()
        {
            bool absorbed = false;
            foreach (var resistor in ArmorResistors)
            {
                if (resistor.Resist(Damage))
                {
                    absorbed = true;
                    break;
                }
            }

            if (!absorbed)
            {
                foreach (var layer in Layers)
                {
                    foreach (var injury in CheckTissueDamage(layer))
                    {
                        yield return injury;
                    }

                    var resistor = LayerResistors[layer];
                    if (resistor.Resist(Damage))
                    {
                        absorbed = true;
                        break;
                    }
                }
            }

            if (!absorbed)
            {
                yield return InjuryFactory.Create(
                    StandardInjuryClasses.MissingBodyPart,
                    BodyPart);
            }
        }
    }
}
