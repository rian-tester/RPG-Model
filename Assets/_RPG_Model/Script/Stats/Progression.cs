using System.Collections.Generic;
using UnityEngine;

namespace RPG.Stats
{
    [CreateAssetMenu(fileName = "NewProgression", menuName = "Test-RPG/Progression/New Progression", order = 0)]
    public class Progression : ScriptableObject
    {
        [SerializeField] ProgressionCharacterClass[] characterClasses = null;

        Dictionary<CharacterClass, Dictionary<Stat, float[]>> lookupTable = null;


        public float GetStats(Stat stat, CharacterClass characterClass, int level)
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

            float[] levels = lookupTable[characterClass][stat];

            if (levels.Length < level) return 0;

            return levels[level -1];
             

        }

        public int GetLevels(Stat stat, CharacterClass characterClass)
        {
            BuildLookup();

            float[] levels = lookupTable[characterClass][stat];
            return levels.Length;
        }

        void BuildLookup()
        {
            if (lookupTable != null) return;

            lookupTable = new Dictionary<CharacterClass, Dictionary<Stat, float[]>>();

            foreach( ProgressionCharacterClass progressionClass in characterClasses ) 
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
        class ProgressionCharacterClass
        {
            public CharacterClass characterClass;
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
