using System.Collections.Generic;
using UnityEngine;

namespace RPG.Stats
{
    [CreateAssetMenu(fileName = "NewProgression", menuName = "Test-RPG/Progression/New Progression", order = 0)]
    public class Progression : ScriptableObject
    {
        [SerializeField] CharacterProgression[] characterProgression = null;

        Dictionary<CharacterType, Dictionary<Stat, float[]>> lookupTable = null;


        // Find the array of levels for the given character Type in specific stat 
        public float GetStats(Stat stat, CharacterType characterClass, int level)
        {
            BuildLookup();
            #region OldLogic
            //foreach (ProgressionCharacterClass progressionClass in characterClasses)
            //{
            //    if (progressionClass.characterClass != characterClass) continue;

            //    foreach(ProgressionStat progressionStat in progressionClass.stats)
            //    {
            //        if (progressionStat.stat != stat) continue;

            //        if (progressionStat.levels.Length < level) continue;

            //        return progressionStat.levels[level -1];
            //    }

            //}
            //Debug.LogWarning("No stats found");
            //return 0f;
            #endregion

            // cached the array of levels for the given character Type in specific stat
            float[] levels = lookupTable[characterClass][stat];

            // check if the requested level is higher than available level in the array
            if (levels.Length < level) return 0;

            // return the array of level to be used
            return levels[level -1];
             

        }

        // find the max level in the CharacterProgression data
        public int GetLevels(Stat stat, CharacterType characterClass)
        {
            BuildLookup();

            float[] levels = lookupTable[characterClass][stat];
            return levels.Length;
        }

        void BuildLookup()
        {
            if (lookupTable != null) return;

            lookupTable = new Dictionary<CharacterType, Dictionary<Stat, float[]>>();

            foreach( CharacterProgression progressionClass in characterProgression ) 
            {
                var statLookupTable = new Dictionary<Stat, float[]>();

                foreach(ProgressionStat progressionStat in progressionClass.stats)
                {
                    statLookupTable[progressionStat.stat] = progressionStat.levels;
                }

                lookupTable[progressionClass.characterClass] = statLookupTable;
            }
        }

        [System.Serializable]
        class CharacterProgression
        {
            public CharacterType characterClass;
            public ProgressionStat[] stats;
        }

        [System.Serializable]
        public class ProgressionStat
        {
            public Stat stat;           
            public float[] levels;
        }

    }

    
}
