using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Materials
{
    public class Material : IMaterial
    {
        public string Adjective { get; private set; }

        public Material(string adjective)
        {
            Adjective = adjective;
        }
    }

    public interface IMaterial
    {
        string Adjective { get; }
    }

    public class TissueLayer : ITissueLayer
    {
        public IMaterial Material { get; private set; }
        public int RelativeThickness { get; private set; }
    }

    public interface ITissueLayer
    {
        IMaterial Material { get; }
        int RelativeThickness { get; }
    }

    public interface ITissue
    {
        IList<ITissueLayer> TissueLayers { get; }
    }

    public class Tissue : ITissue
    {
        public IList<ITissueLayer> TissueLayers { get; private set; }
    }
}
