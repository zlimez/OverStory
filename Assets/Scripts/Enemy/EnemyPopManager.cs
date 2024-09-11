using System.Collections.Generic;
using System.IO;
using Abyss.Utils;
using Tuples;
using UnityEngine;

namespace Enemy
{
    public class EnemyPopManager : Singleton<EnemyPopManager>
    {
        public Triplet<SpecyAttr, GameObject, int>[] enemyPrefabsAndCounts;
        readonly GA _ga = new();
        // string for enemy type, list contains all the enemy for that type
        Dictionary<string, List<EnemyAttr>> _allEnemies;
        Dictionary<string, List<EnemyAttr>> _freeEnemies;
        Dictionary<string, SpecyAttr> _speciesConfig;

        protected override void Awake()
        {
            base.Awake();
            if (!LoadNPCs())
            {
                _allEnemies = new();
                _freeEnemies = new();

                foreach (var enemyPrefabAndCount in enemyPrefabsAndCounts)
                {
                    SpecyAttr enemyAttrSO = enemyPrefabAndCount.Item1;
                    int enemyCount = enemyPrefabAndCount.Item3;
                    _speciesConfig[enemyAttrSO.specyName] = enemyAttrSO;

                    _allEnemies[enemyAttrSO.specyName] = new();
                    _freeEnemies[enemyAttrSO.specyName] = new();
                    var attrRanges = enemyAttrSO.AllAttrRanges();
                    float[,] popDNA = _ga.GetInitPopulation(enemyCount, attrRanges, attrRanges.Length);

                    for (int i = 0; i < enemyCount; i++)
                    {
                        float[] dna = new float[attrRanges.Length];
                        for (int j = 0; j < attrRanges.Length; j++) dna[j] = popDNA[i, j];
                        EnemyAttr enemyAttr = new();
                        enemyAttr.UseDNA(dna);
                        _allEnemies[enemyAttrSO.specyName].Add(enemyAttr);
                        _freeEnemies[enemyAttrSO.specyName].Add(enemyAttr);
                    }
                }
            }
            else
            {
                foreach (var enemyPrefabAndCount in enemyPrefabsAndCounts)
                {
                    SpecyAttr enemyAttrSO = enemyPrefabAndCount.Item1;
                    _speciesConfig[enemyAttrSO.specyName] = enemyAttrSO;
                }
            }
        }

        public void SaveNPCs(string fileName = "npcData.json")
        {
            string json = JsonUtility.ToJson(_allEnemies, true);
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
                _allEnemies = JsonUtility.FromJson<Dictionary<string, List<EnemyAttr>>>(json);

                foreach (var enemyType in _allEnemies)
                {
                    _freeEnemies[enemyType.Key] = new();
                    foreach (var enemyAttr in enemyType.Value) _freeEnemies[enemyType.Key].Add(enemyAttr);
                }
                Debug.Log("NPC data loaded from " + filePath);
                return true;
            }
            else Debug.LogWarning("No NPC data file found at " + filePath);
            return false;
        }

        public void NextGeneration()
        {
            foreach (var enemyType in _allEnemies)
            {
                var specyName = enemyType.Key;
                var enemies = enemyType.Value;
                var specyConfig = _speciesConfig[specyName];
                var chromoLen = specyConfig.attrRanges.Length;
                float[,] parentsDNA = new float[enemies.Count, chromoLen];
                for (int i = 0; i < enemies.Count; i++)
                {
                    float[] dna = enemies[i].DNA;
                    for (int j = 0; j < chromoLen; j++) parentsDNA[i, j] = dna[j];
                }

                float[,] childrenDNA = _ga.GetChildren(parentsDNA, specyConfig.mutationRate, specyConfig.AllAttrRanges(), Mathf.Min(specyConfig.maxPopulation / enemies.Count, specyConfig.generationRate));
                enemies.Clear();
                _freeEnemies[specyName].Clear();

                for (int i = 0; i < childrenDNA.GetLength(0); i++)
                {
                    float[] dna = new float[chromoLen];
                    for (int j = 0; j < chromoLen; j++) dna[j] = childrenDNA[i, j];
                    EnemyAttr enemy = new();
                    enemy.UseDNA(dna);
                    enemies.Add(enemy);
                    _freeEnemies[specyName].Add(enemy);
                }
            }
        }
    }
}
