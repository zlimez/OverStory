using System.Collections;
using System.Collections.Generic;
using System.IO;
using Abyss.Utils;
using UnityEngine;

namespace Enemy
{
    public class EnemyManager : Singleton<EnemyManager>
    {
        private readonly GA _ga = new();
        // string for enemy type, list contains all the enemy for that type
        private Dictionary<string, List<EnemyAttr>> _enemies = new();

        public void SaveNPCs(string fileName = "npcData.json")
        {
            string json = JsonUtility.ToJson(_enemies, true);
            File.WriteAllText(Path.Combine(Application.persistentDataPath, fileName), json);
            Debug.Log("NPC data saved to " + Application.persistentDataPath + "/" + fileName);
        }

        /// <summary>
        /// load NPC data from file
        /// </summary>
        public void LoadNPCs(string fileName = "npcData.json")
        {

            string filePath = Path.Combine(Application.persistentDataPath, fileName);
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                _enemies = JsonUtility.FromJson<Dictionary<string, List<EnemyAttr>>>(json);
                Debug.Log("NPC data loaded from " + filePath);
            }
            else Debug.LogWarning("No NPC data file found at " + filePath);
        }

        public static List<EnemyAttr> NextGeneration(List<EnemyAttr> oldGeneration)
        {
            List<EnemyAttr> nextGeneration = new();

            // float[,] parents = new float[oldGeneration.Count, 2];
            // for (int i = 0; i < oldGeneration.Count; i++)
            // {
            //     float[] dna = oldGeneration[i].GetDNA();
            //     for (int j = 0; j < 2; j++) parents[i, j] = dna[j];
            // }

            // float[,] generations = ga.GetGenerations(parents, 2);
            // for (int i = 0; i < generations.GetLength(0); i++)
            // {
            //     float[] dna = new float[generations.GetLength(1)];
            //     for (int j = 0; j < generations.GetLength(1); j++) dna[j] = generations[i, j];
            //     EnemyAttr npc = new();
            //     npc.UseDNA(dna);
            //     nextGeneration.Add(npc);
            // }

            return nextGeneration;
        }
    }
}
