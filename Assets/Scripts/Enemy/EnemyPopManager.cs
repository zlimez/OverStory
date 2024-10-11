using System;
using System.Collections.Generic;
using System.IO;
using Abyss.EventSystem;
using Abyss.Utils;
using Algorithms;
using Tuples;
using UnityEngine;
using UnityEngine.Assertions;

namespace Abyss.Environment.Enemy
{
    // NOTE: Population for each specy should be large compared to the number of spawning locations
    public class EnemyPopManager : PersistentSystem<EnemyPopManager>
    {
        public static readonly GameEvent EnemyPopManagerReady = new("EnemyPopManager Ready");
        [Tooltip("Species attributes and initial population count")]
        public Pair<SpecyAttr, int>[] SpeciesAttrAndInitCount;
        readonly GA _ga = new();
        // str enemy type, 1st int for genpop count, instances before 2nd int pointer spawned for scene, list contains all instances for that type
        // only first pointer number of instance could be dead killed by player in scene
        Dictionary<string, RefTriplet<int, int, List<EnemyAttr>>> _allEnemies;
        readonly Dictionary<string, SpecyAttr> _speciesConfig = new();

        protected override void Awake()
        {
            base.Awake();
            if (!LoadNPCs())
            {
                _allEnemies = new();

                foreach (var enemyPrefabAndCount in SpeciesAttrAndInitCount)
                {
                    SpecyAttr enemyAttrSO = enemyPrefabAndCount.Head;
                    int enemyCount = enemyPrefabAndCount.Tail;
                    _speciesConfig.Add(enemyAttrSO.specyName, enemyAttrSO);

                    _allEnemies.Add(enemyAttrSO.specyName, new(enemyCount, 0, new()));
                    var attrRanges = enemyAttrSO.AllAttrRanges();
                    float[,] popDNA = _ga.GetInitPopulation(enemyCount, attrRanges, attrRanges.Length);

                    for (int i = 0; i < enemyCount; i++)
                    {
                        float[] dna = new float[attrRanges.Length];
                        for (int j = 0; j < attrRanges.Length; j++) dna[j] = popDNA[i, j];
                        EnemyAttr enemyAttr = new();
                        enemyAttr.UseDNA(dna);
                        _allEnemies[enemyAttrSO.specyName].Item3.Add(enemyAttr);
                    }
                }
            }
            else
            {
                foreach (var specyAttrAndCount in SpeciesAttrAndInitCount)
                {
                    SpecyAttr specyAttr = specyAttrAndCount.Head;
                    _speciesConfig.Add(specyAttr.specyName, specyAttr);
                }
            }
            EventManager.InvokeEvent(EnemyPopManagerReady);
            IsReady = true;
        }

        public void SaveNPCs(string fileName = "npcData.json")
        {
            Dictionary<string, List<EnemyAttr>> aliveEnemies = new();
            foreach (var specyPop in _allEnemies)
            {
                aliveEnemies.Add(specyPop.Key, new());
                foreach (var enemy in specyPop.Value.Item3)
                {
                    if (enemy.isAlive)
                        aliveEnemies[specyPop.Key].Add(enemy);
                }
            }
            string json = JsonUtility.ToJson(aliveEnemies, true);
            File.WriteAllText(Path.Combine(Application.persistentDataPath, fileName), json);
            Debug.Log("NPC data saved to " + Application.persistentDataPath + "/" + fileName);
        }

        /// <summary>
        /// load NPC data from file
        /// </summary>
        bool LoadNPCs(string fileName = "npcData.json")
        {
            string filePath = Path.Combine(Application.persistentDataPath, fileName);
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                var allEnemies = JsonUtility.FromJson<Dictionary<string, List<EnemyAttr>>>(json);

                foreach (var enemyType in allEnemies)
                    _allEnemies.Add(enemyType.Key, new(enemyType.Value.Count, 0, enemyType.Value));

                Debug.Log("NPC data loaded from " + filePath);
                return true;
            }
            else Debug.Log("No NPC data file found at " + filePath);
            return false;
        }

        public void NextGeneration()
        {
            foreach (var enemyType in _allEnemies)
            {
                var specyName = enemyType.Key;
                var enemies = enemyType.Value;
                var specyConfig = _speciesConfig[specyName];
                var parentPop = enemies.Item3.Count;
                var chromoLen = specyConfig.attrRanges.Length;
                float[,] parentsDNA = new float[parentPop, chromoLen];
                int aliveSpawnedPop = enemies.Item2;

                for (int i = 0; i < parentPop; i++)
                {
                    var enemy = enemies.Item3[i];
                    if (!enemy.isAlive)
                    {
                        parentPop--;
                        aliveSpawnedPop--;
                        continue;
                    }
                    for (int j = 0; j < chromoLen; j++) parentsDNA[i, j] = enemy.DNA[j];
                }

                float[,] childrenDNA = _ga.GetChildren(parentsDNA, specyConfig.mutationRate, specyConfig.AllAttrRanges(), Mathf.Min((float)specyConfig.maxPopulation / parentPop, specyConfig.generationRate));
                enemies.Item3.RemoveRange(enemies.Item2, enemies.Item3.Count - enemies.Item2);
                enemies.Item1 = childrenDNA.GetLength(0);

                // Alive spawned enemies in the active scene remains as part of pop, first k children discarded
                for (int i = aliveSpawnedPop; i < childrenDNA.GetLength(0); i++)
                {
                    float[] dna = new float[chromoLen];
                    for (int j = 0; j < chromoLen; j++) dna[j] = childrenDNA[i, j];
                    EnemyAttr enemy = new();
                    enemy.UseDNA(dna);
                    enemies.Item3.Add(enemy);
                }

#if DEBUG
                int activePop = 0;
                foreach (var specyInstance in enemies.Item3)
                    if (specyInstance.isAlive) activePop++;
                Assert.IsTrue(activePop == childrenDNA.GetLength(0));
                Debug.Log("Next generation for " + specyName + " with " + parentPop + " parents and " + childrenDNA.GetLength(0) + " children");
#endif
            }
        }

        // Invoked at the start of each scene by the SpawnManager
        public List<EnemyAttr> GetSpecyInstances(string specyName, int k)
        {
            var specyPop = _allEnemies[specyName].Item3;
            int bpt = specyPop.Count;

            for (int i = 0; i < Math.Min(_allEnemies[specyName].Item2, specyPop.Count); i++)
            {
                var enemy = specyPop[i];
                if (!enemy.isAlive)
                {
                    while (bpt > i && !specyPop[--bpt].isAlive) ;
                    if (bpt == i) break;
                    // Debug.Log("Swapping dead enemy at " + i + " with alive enemy at " + bpt);
                    (specyPop[i], specyPop[bpt]) = (specyPop[bpt], specyPop[i]);
                }
            }
            specyPop.RemoveRange(bpt, specyPop.Count - bpt);

#if DEBUG
            foreach (var specyInstance in specyPop) Assert.IsTrue(specyInstance.isAlive);
#endif

            int nk = Math.Min(k, specyPop.Count);
            ListUtils.ShuffleForK(nk, specyPop);
            _allEnemies[specyName].Item2 = nk;
            System.Random ran = new();

            // Some spawn locations might not have an instance
            List<EnemyAttr> specyInstances = new();
            for (int i = 0; i < k; i++)
            {
                if (i >= nk || (float)ran.NextDouble() >= (float)_allEnemies[specyName].Item1 / _speciesConfig[specyName].maxPopulation) specyInstances.Add(null);
                else specyInstances.Add(specyPop[i]);
            }

            return specyInstances;
        }
    }
}
