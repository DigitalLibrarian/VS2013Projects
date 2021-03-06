﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Math;

namespace Tiles
{
    public interface ISiteFactory
    {
        ISite Create(IAtlas atlas, Vector3 siteIndex, Box3 box);
    }
}
