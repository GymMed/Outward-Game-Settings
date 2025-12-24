using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutwardGameSettings.Utility.Enums
{
    // Too early to release
    public enum AIClassTypes
    {
        Warrior,
        Archer,
        Gunner,
        PassiveMage
    }

    public static class AIClassTypesHelper
    {
        public static readonly Dictionary<AIClassTypes, string> Data = new()
        {
        };
    }
}
