using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Items;

namespace Tiles.Bodies
{
    public class BodyFactory : IBodyFactory
    {
        public IBody CreateHumanoid()
        {
            var torso = new BodyPart(new BodyPartClass("torso", false, false, false, ArmorSlot.Torso, WeaponSlot.None));
            var head = new BodyPart(new BodyPartClass("head", true, true, false, ArmorSlot.Head, WeaponSlot.None), torso);

            var leftArm = new BodyPart(new BodyPartClass("left arm", false, true, false, ArmorSlot.LeftArm, WeaponSlot.None), torso);
            var leftHand = new BodyPart(new BodyPartClass("left hand", false, true, true, ArmorSlot.LeftHand, WeaponSlot.None), leftArm);

            var rightArm = new BodyPart(new BodyPartClass("right arm", false, true, false, ArmorSlot.RightArm, WeaponSlot.None), torso);
            var rightHand = new BodyPart(new BodyPartClass("right hand", false, true, true, ArmorSlot.RightHand, WeaponSlot.Main), rightArm);

            var leftLeg = new BodyPart(new BodyPartClass("left leg", false, true, false, ArmorSlot.LeftLeg, WeaponSlot.None), torso);
            var leftFoot = new BodyPart(new BodyPartClass("left foot", false, true, false, ArmorSlot.LeftFoot, WeaponSlot.None), leftLeg);

            var rightLeg = new BodyPart(new BodyPartClass("right leg", false, true, false, ArmorSlot.RightLeg, WeaponSlot.None), torso);
            var rightFoot = new BodyPart(new BodyPartClass("right foot", false, true, false, ArmorSlot.RightFoot, WeaponSlot.None), rightLeg);

            var bodyParts = new List<IBodyPart>{
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
            return new Body(bodyParts);
        }

        public IBody CreateFeralHumanoid()
        {
            var torso = new BodyPart(new BodyPartClass("torso", false, false, false, ArmorSlot.Torso, WeaponSlot.None));
            var head = new BodyPart(new BodyPartClass("head", true, true, false, ArmorSlot.Head, WeaponSlot.Teeth), torso);

            var leftArm = new BodyPart(new BodyPartClass("left arm", false, true, false, ArmorSlot.LeftArm, WeaponSlot.None), torso);
            var leftHand = new BodyPart(new BodyPartClass("left hand", false, true, true, ArmorSlot.LeftHand, WeaponSlot.Claw), leftArm);

            var rightArm = new BodyPart(new BodyPartClass("right arm", false, true, false, ArmorSlot.RightArm, WeaponSlot.None), torso);
            var rightHand = new BodyPart(new BodyPartClass("right hand", false, true, true, ArmorSlot.RightHand, WeaponSlot.Claw), rightArm);

            var leftLeg = new BodyPart(new BodyPartClass("left leg", false, true, false, ArmorSlot.LeftLeg, WeaponSlot.None), torso);
            var leftFoot = new BodyPart(new BodyPartClass("left foot", false, true, false, ArmorSlot.LeftFoot, WeaponSlot.None), leftLeg);

            var rightLeg = new BodyPart(new BodyPartClass("right leg", false, true, false, ArmorSlot.RightLeg, WeaponSlot.None), torso);
            var rightFoot = new BodyPart(new BodyPartClass("right foot", false, true, false, ArmorSlot.RightFoot, WeaponSlot.None), rightLeg);

            var bodyParts = new List<IBodyPart>{
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
            return new Body(bodyParts);
        }
    }
}
