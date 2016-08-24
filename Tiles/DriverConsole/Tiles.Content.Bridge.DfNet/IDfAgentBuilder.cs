﻿using DfNet.Raws;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiles.Content.Models;

namespace Tiles.Content.Bridge.DfNet
{
    public interface IDfAgentBuilder
    {
        void AddBody(string name, DfObject bpObject);


        void AddMaterial(string matName, Material material);
        void RemoveMaterial(string matName);

        void AddTissueToBodyPart(string bpName, string tisName);

        void SetBodyPartTissueThickness(string bpName, string tisName, int relThick);

        Agent Build();
    }
}