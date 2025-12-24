using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutwardGameSettings.Utility.Helpers
{
    public static class AreasHelper
    {
        public static AreaManager.AreaEnum[] Towns = new AreaManager.AreaEnum[]
        {
            AreaManager.AreaEnum.CierzoVillage,
            AreaManager.AreaEnum.Berg,
            AreaManager.AreaEnum.Monsoon,
            AreaManager.AreaEnum.Levant,
            AreaManager.AreaEnum.NewSirocco,
            AreaManager.AreaEnum.Harmattan,
        };

        public static AreaManager.AreaEnum[] OpenAreas = new AreaManager.AreaEnum[]
        {
            AreaManager.AreaEnum.CierzoOutside,
            AreaManager.AreaEnum.Emercar,
            AreaManager.AreaEnum.HallowedMarsh,
            AreaManager.AreaEnum.Abrassar,
            AreaManager.AreaEnum.Caldera,
            AreaManager.AreaEnum.AntiqueField,
        };

        public static bool IsCurrentAreaInOpenWorld()
        {
            return IsAreaInOpenWorld(AreaManager.Instance.CurrentArea);
        }

        public static bool IsAreaInOpenWorld(Area area)
        {
            return IsAreaInEnumList(area, OpenAreas);
        }

        public static bool IsAreaInEnumList(Area area, IList<AreaManager.AreaEnum> list)
        {
            AreaManager.AreaEnum areaEnum = GetAreaEnumFromArea(area);
            return list.Contains(areaEnum);
        }

        public static AreaManager.AreaEnum GetAreaEnumFromArea(Area area)
        {
            return (AreaManager.AreaEnum)area.ID;
        }
    }
}
