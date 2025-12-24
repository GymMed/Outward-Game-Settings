using OutwardGameSettings.Utility.Enemies.Visuals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutwardGameSettings.Utility.Enums
{
    public enum Races
    {
        Auraian,
        Tramon,
        Kazite
    }

    public static class RacesHelper
    {
        public static readonly Dictionary<Races, RaceData> races = new()
        {
            { Races.Auraian, new RaceData(
                //male
                new GenderData(totalHairStyles : 15, totalHairColors : 11, totalHeadVariations : 8),
                //female
                new GenderData(15, 11, 6)
            ) },

            { Races.Tramon, new(
                new(15, 11, 6),
                new(15, 11, 6)
            ) },

            { Races.Kazite, new(
                new(15, 11, 7),
                new(15, 11, 6)
            ) }
        };
    }
}
