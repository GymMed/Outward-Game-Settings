using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace OutwardGameSettings.Utility.Helpers
{
    public class ConfigsHelper
    {
        public static int GetPercentageValueFromConfig(int originalValue)
        {
            if (originalValue < 0)
                return 0;

            if (originalValue > 100)
                return 100;

            return originalValue;
        }
    }
}
