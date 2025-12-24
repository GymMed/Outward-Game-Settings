using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutwardGameSettings.Utility.Enemies.Visuals
{
    public struct RaceData
    {
        public GenderData Male;
        public GenderData Female;

        public RaceData(
            GenderData male,
            GenderData female
        )
        {
            Male = male;
            Female = female;
        }

        public GenderData GetGender(Character.Gender gender)
        {
            if (gender == Character.Gender.Male)
                return Male;

            return Female;
        }
    }
}
