using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Content.Models
{
    public class Body
    {
        public Body()
        {
            Parts = new List<BodyPart>();
        }
        public List<BodyPart> Parts { get; set; }

        public int Size { get; set; }
    }
}
