using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutwardGameSettings.Managers.Testing
{
    public class TestingKeyBindings
    {
        private static TestingKeyBindings _instance;

        private TestingKeyBindings()
        {
        }

        public static TestingKeyBindings Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new TestingKeyBindings();

                return _instance;
            }
        }

        // needs to live inside singleton instead of static class
        // separated from EnemiesWaveTypesHelper ...
        public static readonly string[] EnemiesKeyBindings = new string[]
        {
            "Spawn Bandits",
            "Spawn Troglodytes",
            "Spawn Skeletons",
            "Spawn Ghosts",

            "Spawn Dummies",
            "Spawn Archer Bandits",
            "Spawn Archer Skeletons",
            "Spawn Gunner Bandits",
            "Spawn Gunner Skeletons",
        };
    }
}
