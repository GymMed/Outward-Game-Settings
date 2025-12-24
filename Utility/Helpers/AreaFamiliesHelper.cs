using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutwardGameSettings.Utility.Helpers
{
    public static class AreaFamiliesHelper
    {
        public static bool IsAreaInAreaFamily(AreaManager.AreaEnum area)
        {
            Area currentArea = AreaManager.Instance.GetArea(area);

            if (currentArea == null)
                return false;

            foreach(AreaFamily areaFamily in AreaManager.AreaFamilies)
            {
                foreach(string familyKeyWord in areaFamily.FamilyKeywords)
                {
                    if(currentArea.SceneName.Contains(familyKeyWord))
                        return true;
                }
            }

            return false;
        }
    }
}
