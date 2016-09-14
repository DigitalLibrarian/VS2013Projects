﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents.Combat;
using Tiles.Bodies;
using Tiles.Items;
using Tiles.Materials;

namespace Tiles.ScreensImpl.ContentFactories
{
    public class BodyClassFactory : IBodyClassFactory
    {
        ITissueClass CreateTissues()
        {
            return new TissueClass(new List<ITissueLayerClass>
            {
                new TissueLayerClass(new Material("bone", "bone"), 25, false),
                new TissueLayerClass(new Material("bone", "fat"), 15, false),
                new TissueLayerClass(new Material("bone", "skin"), 5, false),
            });
        }

        Dictionary<string, BodyPartClass> Human(ITissueClass tissueClass)
        {
            var torso = new BodyPartClass(
                "torso", tissueClass, ArmorSlot.Torso, WeaponSlot.None, new List<string>(), new List<string>(), 1000,
                canBeAmputated: false);

            var head = new BodyPartClass("head", tissueClass, ArmorSlot.Head, WeaponSlot.None, new List<string>(), new List<string>(), 300,
                parent: torso,
                canBeAmputated: true,
                isCritical: true);

            var leftArm = new BodyPartClass("left arm", tissueClass, ArmorSlot.LeftArm, WeaponSlot.None, new List<string>(), new List<string>(), 400,
                parent: torso,
                canBeAmputated: true);

            var leftHand = new BodyPartClass("left hand", tissueClass, ArmorSlot.LeftHand, WeaponSlot.None, new List<string>(), new List<string>(), 80,
                parent: leftArm,
                canGrasp: true,
                canBeAmputated: true);

            var rightArm = new BodyPartClass("right arm", tissueClass, ArmorSlot.RightArm, WeaponSlot.None, new List<string>(), new List<string>(), 400,
                parent: torso,
                canBeAmputated: true);

            var rightHand = new BodyPartClass("right hand", tissueClass, ArmorSlot.RightHand, WeaponSlot.Main, new List<string>(), new List<string>(), 80,
                parent: rightArm,
                canGrasp: true,
                canBeAmputated: true);

            var leftLeg = new BodyPartClass("left leg", tissueClass, ArmorSlot.LeftLeg, WeaponSlot.None, new List<string>(), new List<string>(), 900,
                parent: torso,
                canBeAmputated: true);

            var leftFoot = new BodyPartClass("left foot", tissueClass, ArmorSlot.LeftFoot, WeaponSlot.None, new List<string>(), new List<string>(), 120,
                parent: leftLeg,
                canBeAmputated: true);

            var rightLeg = new BodyPartClass("right leg", tissueClass, ArmorSlot.RightLeg, WeaponSlot.None, new List<string>(), new List<string>(), 900,
                parent: torso,
                canBeAmputated: true);

            var rightFoot = new BodyPartClass("right foot", tissueClass, ArmorSlot.RightFoot, WeaponSlot.None, new List<string>(), new List<string>(), 120,
                parent: rightLeg,
                canBeAmputated: true);

            var pD = new Dictionary<string, BodyPartClass>();

            foreach(var bc in new List<BodyPartClass>(){
                head,
                torso,
                leftArm,
                rightArm,
                leftHand,
                rightHand,
                leftLeg,
                rightLeg,
                leftFoot,
                rightFoot
            })
            {
                pD.Add(bc.Name, bc);
            }
            return pD;
        }

        public IBodyClass CreateHumanoid()
        {
            var tissueClass = CreateTissues();
            var bodyParts = Human(tissueClass).Values.ToList();
            return new BodyClass(60000, bodyParts);
        }

        public IBodyClass CreateFeralHumanoid()
        {
            var tissueClass = CreateTissues();
            var bodyParts = Human(tissueClass);
            bodyParts["head"].WeaponSlot = WeaponSlot.Teeth;
            bodyParts["left hand"].WeaponSlot = WeaponSlot.Claw;
            bodyParts["right hand"].WeaponSlot = WeaponSlot.Claw;
            return new BodyClass(60000, bodyParts.Values.ToList());
        }
    }
}
