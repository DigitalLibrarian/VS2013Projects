using DfNet.Raws;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Content.Models;
namespace Tiles.Content.Bridge.DfNet
{
    public class DfMaterialFactory : IDfMaterialFactory
    {
        IDfObjectStore Store { get; set; }

        public DfMaterialFactory(IDfObjectStore store)
        {
            Store = store;
        }

        public Material CreateInorganic(string name)
        {
            var df = Store.Get(DfTags.INORGANIC, name);
            return Common(df);
        }

        public Material CreateTissue(string tissueTemplate)
        {
            var df = Store.Get(DfTags.TISSUE_TEMPLATE, tissueTemplate);
            return Common(df);
        }


        public Material CreateFromMaterialTemplate(string materialTemplate)
        {
            var df = Store.Get(DfTags.MATERIAL_TEMPLATE, materialTemplate);
            return Common(df);
        }

        private Material Common(DfObject df)
        {
            return new Material
            {
                Adjective = GetMaterialAdj(df)
            };
        }
        string GetMaterialAdj(DfObject matDefn)
        {
            var adjTag = matDefn.Tags.LastOrDefault(
                t => t.Name.Equals(DfTags.MiscTags.TISSUE_NAME));
            if (adjTag != null)
            {
                return adjTag.GetParam(0);
            }

            adjTag = matDefn.Tags.LastOrDefault(
                t => t.Name.Equals(DfTags.MiscTags.STATE_NAME_ADJ)
                && t.GetParam(0).Equals(DfTags.MiscTags.ALL_SOLID));

            if (adjTag != null)
            {
                return adjTag.GetParam(1);
            }

            adjTag = matDefn.Tags.LastOrDefault(
                t => t.Name.Equals(DfTags.MiscTags.STATE_ADJ)
                && t.GetParam(0).Equals(DfTags.MiscTags.ALL_SOLID));

            if (adjTag != null)
            {
                return adjTag.GetParam(1);
            }

            adjTag = matDefn.Tags.LastOrDefault(
                t => t.Name.Equals(DfTags.MiscTags.STATE_ADJ)
                && t.GetParam(0).Contains(DfTags.MiscTags.SOLID));

            if (adjTag != null)
            {
                return adjTag.GetParam(1);
            }

            adjTag = matDefn.Tags.LastOrDefault(
                t => t.Name.Equals(DfTags.MiscTags.STATE_NAME));

            if (adjTag != null)
            {
                return adjTag.GetParam(1);
            }
            throw new InvalidOperationException("Count not come up with adject for " + matDefn.Tags.First().ToString());
        }
    }
}
