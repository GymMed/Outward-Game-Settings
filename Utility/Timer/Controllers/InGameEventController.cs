using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OutwardGameSettings.Utility.Timer.Controllers
{
    public class InGameEventController
    {
        private float chancePerCyclePercent = 1.3888f; // user input in percent (0–100)
        private int checksPerCycle = 72;

        private float baseChance;
        private float accumulator = 0f;

        public float ChancePerCyclePercent { get => chancePerCyclePercent; set => chancePerCyclePercent = value; }
        public int ChecksPerCycle { get => checksPerCycle; set => checksPerCycle = value; }
        public float Accumulator { get => accumulator; set => accumulator = value; }

        public InGameEventController(float chancePerCyclePercent = 1.3888f, int checksPerCycle = 72)
        {
            CalculateHourlyChances(chancePerCyclePercent, checksPerCycle);
        }

        public void CalculateHourlyChances(float chancePerCyclePercent = 1.3888f)
        {
            ChancePerCyclePercent = chancePerCyclePercent;

            CalculateHourlyChances();
        }

        public void CalculateHourlyChances(float chancePerCyclePercent = 1.3888f, int checksPerCycle = 72)
        {
            ChancePerCyclePercent = chancePerCyclePercent;
            ChecksPerCycle = checksPerCycle;

            CalculateHourlyChances();
        }

        public void CalculateHourlyChances()
        {
            float P = chancePerCyclePercent / 100f;  // convert to fraction
            baseChance = 1f - Mathf.Pow(1f - P, 1f / checksPerCycle);
#if DEBUG
            OutwardGameSettings.LogMessage($"Base hourly chance: {baseChance * 100f:F3}%");
#endif
        }

        public void SimulateCheck()
        {
            Accumulator += baseChance;
            Accumulator = Mathf.Min(Accumulator, 1f); // clamp so it never exceeds 100%
        }

        public bool PassedHourlyCheck()
        {
            Accumulator += baseChance;
            if (UnityEngine.Random.Range(0f, 1f) < Accumulator)
            {
                Accumulator = 0f;
                return true;
            }

            return false;
        }
    }
}
