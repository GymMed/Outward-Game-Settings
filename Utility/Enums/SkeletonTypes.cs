using OutwardGameSettings.Utility.Enemies.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutwardGameSettings.Utility.Enums
{
    public enum SkeletonTypes
    {
        Simple,
        AncientBlue
    }

    public static class SkeletonTypesHelper
    {
        public static readonly Dictionary<SkeletonTypes, SkeletonTypeData> types = new()
        {
            { SkeletonTypes.Simple, new(175, 3200031, 3200030, 3200032)},
            { SkeletonTypes.AncientBlue, new(300, 3300301, 3300300, 3300302)}
        };
    }
}
