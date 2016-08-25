using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                ArmorSlot.Torso, WeaponSlot.None);

            var head = new BodyPartClass(
                "head",
                true, true, false,
                tissueClass,
                ArmorSlot.Head, WeaponSlot.None,
                torso);

            var leftArm = new BodyPartClass(
                "left arm",
                false, true, false,
                tissueClass,
                ArmorSlot.LeftArm, WeaponSlot.None,
                torso);

            var leftHand = new BodyPartClass(
                "left hand",
                false, true, true,
                tissueClass,
                ArmorSlot.LeftHand, WeaponSlot.None,
                leftArm);

            var rightArm = new BodyPartClass(
                "right arm",
                false, true, false,
                tissueClass,
                ArmorSlot.RightArm, WeaponSlot.None,
                torso);

            var rightHand = new BodyPartClass(
                "right hand",
                false, true, true,
                tissueClass,
                ArmorSlot.RightHand, WeaponSlot.Main,
                rightArm);

            var leftLeg = new BodyPartClass(
                "left leg",
                false, true, false,
                tissueClass,
                ArmorSlot.LeftLeg, WeaponSlot.None,
                torso);

            var leftFoot = new BodyPartClass(
                "left foot",
                false, true, false,
                tissueClass,
                ArmorSlot.LeftFoot, WeaponSlot.None,
                leftLeg);

            var rightLeg = new BodyPartClass(
                "right leg",
                false, true, false,
                tissueClass,
                ArmorSlot.RightLeg, WeaponSlot.None,
                torso);

            var rightFoot = new BodyPartClass(
                "right foot",
                false, true, false,
                tissueClass,
                ArmorSlot.RightFoot, WeaponSlot.None,
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
                ArmorSlot.Torso, WeaponSlot.None);

            var head = new BodyPartClass(
                "head",
                true, true, false,
                tissueClass,
                ArmorSlot.Head, WeaponSlot.Teeth, torso);

            var leftArm = new BodyPartClass(
                "left arm",
                false, true, false,
                tissueClass,
                ArmorSlot.LeftArm, WeaponSlot.None,
                torso);

            var leftHand = new BodyPartClass(
                "left hand",
                false, true, true,
                tissueClass,
                ArmorSlot.LeftHand, WeaponSlot.Claw,
                leftArm);

            var rightArm = new BodyPartClass(
                "right arm",
                false, true, false,
                tissueClass,
                ArmorSlot.RightArm, WeaponSlot.None,
                torso);

            var rightHand = new BodyPartClass(
                "right hand",
                false, true, true,
                tissueClass,
                ArmorSlot.RightHand, WeaponSlot.Claw,
                rightArm);

            var leftLeg = new BodyPartClass(
                "left leg",
                false, true, false,
                tissueClass,
                ArmorSlot.LeftLeg, WeaponSlot.None,
                torso);

            var leftFoot = new BodyPartClass(
                "left foot",
                false, true, false,
                tissueClass,
                ArmorSlot.LeftFoot, WeaponSlot.None, leftLeg);

            var rightLeg = new BodyPartClass(
                "right leg",
                false, true, false,
                tissueClass,
                ArmorSlot.RightLeg, WeaponSlot.None,
                torso);

            var rightFoot = new BodyPartClass(
                "right foot",
                false, true, false,
                tissueClass,
                ArmorSlot.RightFoot, WeaponSlot.None,
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
