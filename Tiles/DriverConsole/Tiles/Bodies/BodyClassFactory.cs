using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Agents.Combat;
using Tiles.Items;
using Tiles.Materials;

namespace Tiles.Bodies
{
    public class BodyClassFactory : IBodyClassFactory
    {
        ITissueClass CreateTissues()
        {
            return new TissueClass(new List<ITissueLayerClass>
            {
                new TissueLayerClass(new Material("bone"), 25),
                new TissueLayerClass(new Material("fat"), 15),
                new TissueLayerClass(new Material("skin"), 5),
            });
        }

        public IBodyClass CreateHumanoid()
        {
            var tissueClass = CreateTissues();
            var torso = new BodyPartClass(
                "torso",
                false, false, false,
                tissueClass,
                ArmorSlot.Torso, WeaponSlot.None,
                new List<ICombatMoveClass>());

            var head = new BodyPartClass(
                "head",
                true, true, false,
                tissueClass,
                ArmorSlot.Head, WeaponSlot.None,
                new List<ICombatMoveClass>(),
                torso);

            var leftArm = new BodyPartClass(
                "left arm",
                false, true, false,
                tissueClass,
                ArmorSlot.LeftArm, WeaponSlot.None,
                new List<ICombatMoveClass>(),
                torso);

            var leftHand = new BodyPartClass(
                "left hand",
                false, true, true,
                tissueClass,
                ArmorSlot.LeftHand, WeaponSlot.None,
                new List<ICombatMoveClass>(),
                leftArm);

            var rightArm = new BodyPartClass(
                "right arm",
                false, true, false,
                tissueClass,
                ArmorSlot.RightArm, WeaponSlot.None,
                new List<ICombatMoveClass>(),
                torso);

            var rightHand = new BodyPartClass(
                "right hand",
                false, true, true,
                tissueClass,
                ArmorSlot.RightHand, WeaponSlot.Main,
                new List<ICombatMoveClass>(),
                rightArm);

            var leftLeg = new BodyPartClass(
                "left leg",
                false, true, false,
                tissueClass,
                ArmorSlot.LeftLeg, WeaponSlot.None,
                new List<ICombatMoveClass>(),
                torso);

            var leftFoot = new BodyPartClass(
                "left foot",
                false, true, false,
                tissueClass,
                ArmorSlot.LeftFoot, WeaponSlot.None,
                new List<ICombatMoveClass>(),
                leftLeg);

            var rightLeg = new BodyPartClass(
                "right leg",
                false, true, false,
                tissueClass,
                ArmorSlot.RightLeg, WeaponSlot.None,
                new List<ICombatMoveClass>(),
                torso);

            var rightFoot = new BodyPartClass(
                "right foot",
                false, true, false,
                tissueClass,
                ArmorSlot.RightFoot, WeaponSlot.None,
                new List<ICombatMoveClass>(),
                rightLeg);

            var bodyParts = new List<IBodyPartClass>{
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
            };
            return new BodyClass(bodyParts);
        }

        public IBodyClass CreateFeralHumanoid()
        {
            var tissueClass = CreateTissues();
            var torso = new BodyPartClass(
                "torso",
                false, false, false,
                tissueClass,
                ArmorSlot.Torso, WeaponSlot.None,
                new List<ICombatMoveClass>());

            var head = new BodyPartClass(
                "head",
                true, true, false,
                tissueClass,
                ArmorSlot.Head, WeaponSlot.Teeth,
                new List<ICombatMoveClass>(),
                torso);

            var leftArm = new BodyPartClass(
                "left arm",
                false, true, false,
                tissueClass,
                ArmorSlot.LeftArm, WeaponSlot.None,
                new List<ICombatMoveClass>(),
                torso);

            var leftHand = new BodyPartClass(
                "left hand",
                false, true, true,
                tissueClass,
                ArmorSlot.LeftHand, WeaponSlot.Claw,
                new List<ICombatMoveClass>(),
                leftArm);

            var rightArm = new BodyPartClass(
                "right arm",
                false, true, false,
                tissueClass,
                ArmorSlot.RightArm, WeaponSlot.None,
                new List<ICombatMoveClass>(),
                torso);

            var rightHand = new BodyPartClass(
                "right hand",
                false, true, true,
                tissueClass,
                ArmorSlot.RightHand, WeaponSlot.Claw,
                new List<ICombatMoveClass>(),
                rightArm);

            var leftLeg = new BodyPartClass(
                "left leg",
                false, true, false,
                tissueClass,
                ArmorSlot.LeftLeg, WeaponSlot.None,
                new List<ICombatMoveClass>(),
                torso);

            var leftFoot = new BodyPartClass(
                "left foot",
                false, true, false,
                tissueClass,
                ArmorSlot.LeftFoot, WeaponSlot.None,
                new List<ICombatMoveClass>(), 
                leftLeg);

            var rightLeg = new BodyPartClass(
                "right leg",
                false, true, false,
                tissueClass,
                ArmorSlot.RightLeg, WeaponSlot.None,
                new List<ICombatMoveClass>(),
                torso);

            var rightFoot = new BodyPartClass(
                "right foot",
                false, true, false,
                tissueClass,
                ArmorSlot.RightFoot, WeaponSlot.None,
                new List<ICombatMoveClass>(),
                rightLeg);

            var bodyParts = new List<IBodyPartClass>{
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
            };
            return new BodyClass(bodyParts);
        }
    }
}
