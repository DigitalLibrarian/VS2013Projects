using DfNet.Raws;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Content.Bridge.DfNet
{
    public class DfAgentBuilder : IDfAgentBuilder
    {
        Dictionary<string, DfObject> BodyPartsDefn { get; set; }


        public DfAgentBuilder()
        {
            BodyPartsDefn = new Dictionary<string, DfObject>();
        }

        #region Lookups
        IEnumerable<string> GetBodyPartCategories(DfObject bpObject)
        {
            return bpObject.Tags
                .Where(t => t.Name.Equals(DfTags.MiscTags.CATEGORY))
                .Select(t => t.GetParam(0));
        }

        IEnumerable<DfObject> GetRootPartDefns()
        {
            var parts =  BodyPartsDefn.Values.Where(o => !o.Tags.Any(
                t => t.Name.Equals(DfTags.MiscTags.CONTYPE)
                || t.Name.Equals(DfTags.MiscTags.CON)
                || t.Name.Equals(DfTags.MiscTags.CONCAT)
                ));

            return parts;
        }
        #endregion


        public void AddBody(string name, DfObject bpObject)
        {
//
//[BODY:RCP_TWO_PART_ARMS]
//    [BP:RUA:right upper arm:STP][CONTYPE:UPPERBODY][LIMB][RIGHT][CATEGORY:ARM_UPPER]
//        [DEFAULT_RELSIZE:200]
//    [BP:LUA:left upper arm:STP][CONTYPE:UPPERBODY][LIMB][LEFT][CATEGORY:ARM_UPPER]
//        [DEFAULT_RELSIZE:200]
//    [BP:RLA:right lower arm:STP][CON:RUA][LIMB][RIGHT][CATEGORY:ARM_LOWER]
//    [DEFAULT_RELSIZE:1000]

            var tags = bpObject.Tags.Skip(1);

            var bodyPartDefns = tags
                .Where(t => t.Name.Equals(DfTags.MiscTags.BP))
                .Select<DfTag, List<DfTag>>(t =>
                {
                    var index = bpObject.Tags.IndexOf(t);
                    var count = 1;
                    DfTag next = bpObject.Next(t);
                    while (next != null && !next.Name.Equals(DfTags.MiscTags.BP))
                    {
                        count++;
                        next = bpObject.Next(next);
                    }
                    return bpObject.Tags.ToList().GetRange(index, count);
                })
                .Select(tagList => new DfObject(tagList.ToList()));

            foreach (var bodyPartDefn in bodyPartDefns)
            {
                BodyPartsDefn[bodyPartDefn.Name] = bodyPartDefn;
            }
        }

        public void AddMaterialFromTemplate(string matName, DfObject matObj)
        {

        }

        public void RemoveMaterial(string matName)
        {

        }

        public void AddTissueToBodyPart(string bpName, string tisName)
        {

        }

        public void SetBodyPartTissueThickness(string bpName, string tisName, int relThick)
        {
            throw new NotImplementedException();
        }

        BodyPart Create(DfObject defn)
        {
            return new BodyPart
            {
                NameSingular = defn.Tags.First().GetParam(1),
                NamePlural = defn.Tags.First().GetParam(2)
            };
        }


        public Agent Build()
        {
            var parts = BodyPartsDefn.Values.Select(Create)
                .ToList();

            return new Agent
            {
                Body = new Body
                {
                    Parts = parts
                }
            };
        }
    }
}
