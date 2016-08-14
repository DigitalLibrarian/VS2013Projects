using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Bodies;
using Tiles.Items;

namespace Tiles.ScreensImpl.ContentFactories
{
    public class BodyFactory
    {
        public IBody CreateHumanoid()
        {
            var torso = new BodyPart("torso", false, false, false, ArmorSlot.Torso, WeaponSlot.None);
            var head = new BodyPart("head", true, true, false, ArmorSlot.Head, WeaponSlot.None, torso);

            var leftArm = new BodyPart("left arm", false, true, false, ArmorSlot.LeftArm, WeaponSlot.None, torso);
            var leftHand = new BodyPart("left hand", false, true, true, ArmorSlot.LeftHand, WeaponSlot.None, leftArm);

            var rightArm = new BodyPart("right arm", false, true, false, ArmorSlot.RightArm, WeaponSlot.None, torso);
            var rightHand = new BodyPart("right hand", false, true, true, ArmorSlot.RightHand, WeaponSlot.Main, rightArm);

            var leftLeg = new BodyPart("left leg", false, true, false, ArmorSlot.LeftLeg, WeaponSlot.None, torso);
            var leftFoot = new BodyPart("left foot", false, true, false, ArmorSlot.LeftFoot, WeaponSlot.None, leftLeg);

            var rightLeg = new BodyPart("right leg", false, true, false, ArmorSlot.RightLeg, WeaponSlot.None, torso);
            var rightFoot = new BodyPart("right foot", false, true, false, ArmorSlot.RightFoot, WeaponSlot.None, rightLeg);

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
            var torso = new BodyPart("torso", false, false, false, ArmorSlot.Torso, WeaponSlot.None);
            var head = new BodyPart("head", true, true, false, ArmorSlot.Head, WeaponSlot.Teeth, torso);

            var leftArm = new BodyPart("left arm", false, true, false, ArmorSlot.LeftArm, WeaponSlot.None, torso);
            var leftHand = new BodyPart("left hand", false, true, true, ArmorSlot.LeftHand, WeaponSlot.Claw, leftArm);

            var rightArm = new BodyPart("right arm", false, true, false, ArmorSlot.RightArm, WeaponSlot.None, torso);
            var rightHand = new BodyPart("right hand", false, true, true, ArmorSlot.RightHand, WeaponSlot.Claw, rightArm);

            var leftLeg = new BodyPart("left leg", false, true, false, ArmorSlot.LeftLeg, WeaponSlot.None, torso);
            var leftFoot = new BodyPart("left foot", false, true, false, ArmorSlot.LeftFoot, WeaponSlot.None, leftLeg);

            var rightLeg = new BodyPart("right leg", false, true, false, ArmorSlot.RightLeg, WeaponSlot.None, torso);
            var rightFoot = new BodyPart("right foot", false, true, false, ArmorSlot.RightFoot, WeaponSlot.None, rightLeg);

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
