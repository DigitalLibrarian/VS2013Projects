using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Materials
{
    public interface ILayeredMaterialStrikeResult
    {
        int Penetration { get; }
        IEnumerable<IMaterialStrikeResult> LayerResults { get; }
        IDictionary<object, IMaterialStrikeResult> TaggedResults { get; }

        void AddLayerResult(IMaterialStrikeResult result);
        void AddLayerResult(IMaterialStrikeResult result, object tag);
    }

    public class LayeredMaterialStrikeResult : ILayeredMaterialStrikeResult
    {
        List<IMaterialStrikeResult> Results { get; set; }
        Dictionary<object, IMaterialStrikeResult> Tagged { get; set; }
        public int Penetration { get; set; }
        public LayeredMaterialStrikeResult()
        {
            Results = new List<IMaterialStrikeResult>();
            Tagged = new Dictionary<object, IMaterialStrikeResult>();
        }

        public IEnumerable<IMaterialStrikeResult> LayerResults { get { return Results; } }
        public IDictionary<object, IMaterialStrikeResult> TaggedResults { get { return Tagged; } }

        public void AddLayerResult(IMaterialStrikeResult result)
        {
            Results.Add(result);
        }

        public void AddLayerResult(IMaterialStrikeResult result, object tag)
        {
            AddLayerResult(result);
            Tagged.Add(tag, result);
        }
    }
}
