using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Materials;

namespace Tiles.Bodies
{
    public class BodyClass : IBodyClass
    {
        public BodyClass(IEnumerable<IBodyPartClass> parts)
        {
            Parts = parts;
        }
        public IEnumerable<IBodyPartClass> Parts { get; set; }
    }

    public interface ITissueClass
    {
        IEnumerable<ITissueLayerClass> TissueLayers { get; set; }
    }

    public class TissueClass : ITissueClass
    {
        public TissueClass(IEnumerable<ITissueLayerClass> tLClasses)
        {
            TissueLayers = tLClasses;
        }
        public IEnumerable<ITissueLayerClass> TissueLayers { get; set; }
    }

    public interface ITissueLayerClass
    {
        IMaterial Material { get; }
        int RelativeThickness { get; }
    }

    public class TissueLayerClass : ITissueLayerClass
    {
        public IMaterial Material { get; set; }
        public int RelativeThickness { get; set; }
        public TissueLayerClass(IMaterial material, int relThick)
        {
            Material = material;
            RelativeThickness = relThick;
        }
    }

}
