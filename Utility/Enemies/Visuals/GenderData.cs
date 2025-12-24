using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutwardGameSettings.Utility.Enemies.Visuals
{
    public struct GenderData
    {
        public int TotalHairStyles;
        public int TotalHairColors;
        public int TotalHeadVariations;

        public GenderData(
            int totalHairStyles = 0,
            int totalHairColors = 0,
            int totalHeadVariations = 0
        )
        {
            TotalHairStyles = totalHairStyles;
            TotalHairColors = totalHairColors;
            TotalHeadVariations = totalHeadVariations;
        }
    }
}
