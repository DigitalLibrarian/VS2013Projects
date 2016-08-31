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
            DfMaterialFactory = new DfMaterialFactory(Store, new DfMaterialBuilderFactory());
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
            var iron = DfMaterialFactory.CreateInorganic("IRON");
            Assert.AreEqual("iron", iron.Name);
            Assert.AreEqual("iron", iron.Adjective);
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
            
            /*
            var tissueTemplateNames = Store.Get(DfTags.TISSUE_TEMPLATE).Select(o => o.Name);
            foreach (var tisTempName in tissueTemplateNames)
            {
                CheckProps(DfMaterialFactory.CreateTissue(tisTempName));
            }
            */
            
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

            Assert.AreNotEqual(0, m.ImpactFracture,
                string.Format("ImpactFracture == 0 for Name={0}, Adjective={1}", m.Name, m.Adjective));

            if (m.ImpactFracture == 1)
            {
                Assert.IsTrue(HackedMaterialNames.Contains(m.Name), string.Format("Unknown material property hack for Name={0}, Adjective={1}", m.Name, m.Adjective));
            }
        }
    }
}
