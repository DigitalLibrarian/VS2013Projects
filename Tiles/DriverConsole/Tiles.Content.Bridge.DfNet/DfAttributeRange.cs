﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tiles.Content.Bridge.DfNet
{
    public class DfAttributeRange
    {
        //lowest:lower:low:median:high:higher:highest
        List<int> Values { get; set; }
        public DfAttributeRange(string name, List<int> thresholds)
        {
            if (thresholds.Count < 1)
            {
                throw new InvalidOperationException("Attribute ranges requires 7 thresholds");
            }

            if (thresholds.Count < 7)
            {
                int diff = 7 - thresholds.Count;
                thresholds = thresholds.Concat(
                    Enumerable.Repeat(thresholds.Last(), diff)).ToList();

            }

            Name = name;
            Values = thresholds;
        }

        private const int _ThresholdIndex = 3;
        public int Median { get { return Values[_ThresholdIndex]; } }
        public string Name { get; private set; }
    }
}
