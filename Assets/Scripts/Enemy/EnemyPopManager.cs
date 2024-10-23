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
    public class EnemyPopManager : SystemSingleton<EnemyPopManager>
    {
        [Tooltip("Species attributes and initial population count")] public Pair<SpecyAttr, int>[] SpeciesAttrAndInitCount;
        [SerializeField][Tooltip("Should be multiples of broadcast interval in TimeCycle")] float breedInterval = 12;

        public float FriendlinessAverage;
        public Pair<float, float> FriendlinessRange;

        float _nextBreedTime;
        readonly GA _ga = new();
        // str enemy type, 1st int for genpop count, instances before 2nd int pointer spawned for scene, list contains all instances for that type
        // only first pointer number of instance could be dead killed by player in scene
        Dictionary<string, RefTriplet<int, int, List<EnemyAttr>>> _allEnemies = new();
        readonly Dictionary<string, SpecyAttr> _speciesConfig = new();

        int priorityLevel = 5;

        protected override void Awake()
        {
            base.Awake();
            _nextBreedTime = breedInterval;
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
                        enemyAttr.friendliness = 5.0f;
                        _allEnemies[enemyAttrSO.specyName].Item3.Add(enemyAttr);
                    }
                }
                FriendlinessAverage = 5.0f;
                FriendlinessRange.Head = 0.0f;
                FriendlinessRange.Tail = 10.0f;
            }
            else
            {
                foreach (var specyAttrAndCount in SpeciesAttrAndInitCount)
                {
                    SpecyAttr specyAttr = specyAttrAndCount.Head;
                    _speciesConfig.Add(specyAttr.specyName, specyAttr);
                }
            }
            EventManager.InvokeEvent(SystemEvents.EnemyPopManagerReady);
            IsReady = true;
            EventManager.InvokeEvent(PlayEvents.PlayerFriendlinessPurityChange);
        }

        void OnEnable() => EventManager.StartListening(SystemEvents.TimeBcastEvent, Breed);
        void OnDisable() => EventManager.StopListening(SystemEvents.TimeBcastEvent, Breed);

        void Breed(object input = null)
        {
            (_, float ttTime) = (ValueTuple<float, float>)input;
            if (ttTime < _nextBreedTime) return;
            while (ttTime >= _nextBreedTime)
            {
                NextGeneration();
                _nextBreedTime += breedInterval;
            }
            Debug.Log("Next breed at " + _nextBreedTime);
            // SaveNPCs();
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
            float EnemyCount = 0.0f, FriendlinessCount = 0.0f;
            foreach (var enemyType in _allEnemies)
            {
                var specyName = enemyType.Key;
                var enemies = enemyType.Value;
                var specyConfig = _speciesConfig[specyName];
                var parentPop = enemies.Item3.Count;
                var chromoLen = specyConfig.attrRanges.Length;
                int deathPop = 0, aliveSpawnedPop = enemies.Item2, priorityPop = 0;

                for (int i = 0; i < parentPop; i++)
                {
                    var enemy = enemies.Item3[i];
                    if (!enemy.isAlive) deathPop++;
                    if (enemy.priority) priorityPop++;
                }
                float[,] parentsDNA = new float[parentPop - deathPop + priorityLevel * priorityPop, chromoLen];
                for (int i = 0, j = 0; i < parentPop; i++)
                {
                    var enemy = enemies.Item3[i];
                    if (enemy.isAlive)
                    {
                        for (int k = 0; k < chromoLen; k++) parentsDNA[j, k] = enemy.DNA[k];
                        j++;
                        if (enemy.priority)
                        {
                            for (int ii = 0; ii < priorityLevel; ii++)
                            {
                                for (int k = 0; k < chromoLen; k++) parentsDNA[j, k] = enemy.DNA[k];
                                j++;
                            }
                        }
                    }
                }
                parentPop -= deathPop;

                float[,] childrenDNA = _ga.GetChildren(parentsDNA, specyConfig.mutationRate, specyConfig.AllAttrRanges(), Mathf.Min((float)specyConfig.maxPopulation / parentPop, specyConfig.generationRate));
                enemies.Item3.RemoveRange(enemies.Item2, enemies.Item3.Count - enemies.Item2);
                enemies.Item1 = childrenDNA.GetLength(0);

                // Spawned enemies in the active scene remains as part of pop, first k children discarded, dead ones removed at next get specy instance call
                for (int i = aliveSpawnedPop - deathPop; i < childrenDNA.GetLength(0); i++)
                {
                    float[] dna = new float[chromoLen];
                    for (int j = 0; j < chromoLen; j++) dna[j] = childrenDNA[i, j];
                    EnemyAttr enemy = new();
                    enemy.UseDNA(dna);
                    FriendlinessCount += enemy.friendliness;
                    EnemyCount ++;
                    enemies.Item3.Add(enemy);
                }

#if UNITY_EDITOR
                int activePop = 0;
                foreach (var specyInstance in enemies.Item3)
                    if (specyInstance.isAlive) activePop++;
                Assert.IsTrue(activePop == childrenDNA.GetLength(0));
                Debug.Log("Next generation for " + specyName + " with " + parentPop + " parents and " + childrenDNA.GetLength(0) + " children");
#endif
            }
            FriendlinessAverage = FriendlinessCount / (float) EnemyCount;
            EventManager.InvokeEvent(PlayEvents.PlayerFriendlinessPurityChange);
        }

        // Invoked at the start of each scene by the SpawnManager
        public List<EnemyAttr> GetSpecyInstances(string specyName, int k)
        {
            if (!_allEnemies.ContainsKey(specyName))
            {
                Debug.LogWarning($"Species '{specyName}' not found in _allEnemies.");
                return null;
            }

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
