using DfNet.Raws;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Content.Models;

namespace Tiles.Content.Bridge.DfNet.IntegrationTests
{
    [TestClass]
    public class ContentParsing_MaterialTests
    {
        IDfObjectStore Store { get; set; }
        IDfMaterialFactory DfMaterialFactory { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            Store = TestContentStore.Get();
            DfMaterialFactory = new DfMaterialFactory(Store, new DfMaterialBuilderFactory(), new DfColorFactory());
        }

        [TestMethod]
        public void FlameTemplate()
        {
            var flame = DfMaterialFactory.CreateFromMaterialTemplate("FLAME_TEMPLATE");
            Assert.AreEqual("flames", flame.Name);
            Assert.AreEqual("flames", flame.Adjective);
        }

        [TestMethod]
        public void BoneFromMaterialTemplate()
        {
            var bone = DfMaterialFactory.CreateFromMaterialTemplate("BONE_TEMPLATE");
            Assert.AreEqual(200000, bone.ImpactYield);
        }


        [TestMethod]
        public void Iron()
        {
            var mat = DfMaterialFactory.CreateInorganic("IRON");
            Assert.AreEqual("iron", mat.Name);
            Assert.AreEqual("iron", mat.Adjective);
            Assert.AreEqual(1d, mat.SharpnessMultiplier);

            Assert.AreEqual(542500, mat.ImpactYield);
            Assert.AreEqual(1085000, mat.ImpactFracture);
            Assert.AreEqual(319, mat.ImpactStrainAtYield);

            Assert.AreEqual(542500, mat.CompressiveYield);
            Assert.AreEqual(1085000, mat.CompressiveFracture);
            Assert.AreEqual(319, mat.CompressiveStrainAtYield);

            Assert.AreEqual(155000, mat.TensileYield);
            Assert.AreEqual(310000, mat.TensileFracture);
            Assert.AreEqual(73, mat.TensileStrainAtYield);

            Assert.AreEqual(155000, mat.TorsionYield);
            Assert.AreEqual(310000, mat.TorsionFracture);
            Assert.AreEqual(189, mat.TorsionStrainAtYield);

            Assert.AreEqual(155000, mat.ShearYield);
            Assert.AreEqual(310000, mat.ShearFracture);
            Assert.AreEqual(189, mat.ShearStrainAtYield);

            Assert.AreEqual(155000, mat.BendingYield);
            Assert.AreEqual(310000, mat.BendingFracture);
            Assert.AreEqual(73, mat.BendingStrainAtYield);

            Assert.AreEqual("molten iron", mat.StateProps.Single(sp => sp.Name.Equals("NAME") && sp.State.Equals("LIQUID")).Value);
            Assert.AreEqual("boiling iron", mat.StateProps.Single(sp => sp.Name.Equals("ADJ") && sp.State.Equals("GAS")).Value);
            Assert.AreEqual("GRAY", mat.StateProps.Single(sp => sp.Name.Equals("COLOR") && sp.State.Equals("ALL_SOLID")).Value);

        }

        [TestMethod]
        public void AllMatsHaveProperties()
        {
            var inorganicName = Store.Get(DfTags.INORGANIC).Select(o => o.Name);
            
            foreach (var inorgName in inorganicName)
            {
                var mat = DfMaterialFactory.CreateInorganic(inorgName);
                CheckProps(mat);
            }

            var materialTemplateNames = Store.Get(DfTags.MATERIAL_TEMPLATE).Select(o => o.Name);
            foreach (var matTempName in materialTemplateNames)
            {
                CheckProps(DfMaterialFactory.CreateFromMaterialTemplate(matTempName));
            }
            
            foreach (var creatureDf in Store.Get(DfTags.CREATURE))
            {
                foreach(var inlineTissueTag in creatureDf.Tags.Where(t => t.Name.Equals(DfTags.MiscTags.TISSUE)))
                {
                    CheckProps(DfMaterialFactory.CreateFromTissueCreatureInline(creatureDf.Name, inlineTissueTag.GetParam(0)));
                }
            }
        }

        string[] HackedMaterialNames = new string[]{
            "flames",
            "mud"
        };

        void CheckProps(Material m)
        {
            Assert.IsNotNull(m);
            Assert.IsNotNull(m.Name);
            Assert.IsNotNull(m.Adjective);

            if(HackedMaterialNames.Contains(m.Name))
            {
                Assert.AreEqual(1, m.ImpactFracture, string.Format("Unknown material property hack for Name={0}, Adjective={1}", m.Name, m.Adjective));
                Assert.AreEqual(0, m.SolidDensity);
            }
            else
            {
                Assert.AreNotEqual(0, m.ImpactFracture,
                    string.Format("ImpactFracture == 0 for Name={0}, Adjective={1}", m.Name, m.Adjective));

                Assert.AreNotEqual(0, m.SolidDensity,
                    string.Format("SolidDensity == 0 for Name={0}, Adjective={1}", m.Name, m.Adjective));

                Assert.AreNotEqual(0, m.SharpnessMultiplier,
                    string.Format("Sharpness == 0 for Name={0}, Adjective={1}", m.Name, m.Adjective));

                Assert.IsTrue(m.StateProps.Any(), string.Format("No state properties for Name={0}, Adjective={1}", m.Name, m.Adjective));
            }
        }
    }
}
