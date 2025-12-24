using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutwardGameSettings.Serializable
{
    [Serializable]
    public class EventSaveData
    {
        public int LastDay;
        public int LastHour;

        public List<float> Accumulators = new List<float>();
    }
}
