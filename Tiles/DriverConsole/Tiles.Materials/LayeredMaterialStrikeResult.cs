using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Materials
{
    public interface ILayeredMaterialStrikeResult
    {
        double Penetration { get; }
        IEnumerable<MaterialStrikeResult> LayerResults { get; }
        IDictionary<object, MaterialStrikeResult> TaggedResults { get; }
    }

    public class LayeredMaterialStrikeResult : ILayeredMaterialStrikeResult
    {
        List<MaterialStrikeResult> Results { get; set; }
        Dictionary<object, MaterialStrikeResult> Tagged { get; set; }
        public double Penetration { get; set; }
        public LayeredMaterialStrikeResult()
        {
            Results = new List<MaterialStrikeResult>();
            Tagged = new Dictionary<object, MaterialStrikeResult>();
        }

        public IEnumerable<MaterialStrikeResult> LayerResults { get { return Results; } }
        public IDictionary<object, MaterialStrikeResult> TaggedResults { get { return Tagged; } }

        public void AddLayerResult(MaterialStrikeResult result)
        {
            Results.Add(result);
        }

        public void AddLayerResult(MaterialStrikeResult result, object tag)
        {
            AddLayerResult(result);
            Tagged.Add(tag, result);
        }
    }
}
