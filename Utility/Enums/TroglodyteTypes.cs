using OutwardGameSettings.Utility.Enemies.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutwardGameSettings.Utility.Enums
{
    public enum TroglodyteTypes
    {
        Simple,
        Mana,
        Armored,
        Knight,
        Grenadier,
        Annoying,
        Archmage
    }

    public static class TroglodyteTypesHelper
    {
        public static readonly Dictionary<TroglodyteTypes, TroglodyteTypeData> types = new()
        {
            { TroglodyteTypes.Simple, new(90, 2130080) },//2130083
            { TroglodyteTypes.Mana, new(200, 2150040, 3900000) },
            { TroglodyteTypes.Armored, new(225, 2140050, 3900001) },
            { TroglodyteTypes.Knight, new(275, 2130081, 3900002) },//2130084
            { TroglodyteTypes.Grenadier, new(120, 2140050, 3900003) },
            { TroglodyteTypes.Annoying, new(125, 2140051, 3900004) },
            { TroglodyteTypes.Archmage, new(300, 2150041, 3900005) },
        };
    }
}
