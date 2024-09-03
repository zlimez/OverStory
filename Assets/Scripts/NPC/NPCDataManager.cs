using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;

public class NPCDataManager
{
    private class NPCDataList
    {
        public List<NPCData> npcs;
    }
    static public void SaveNPCs(List<GameObject> npcs, string fileName = "npcData.json")
    {
        List<NPCData> npcDataList = new();

        foreach (GameObject npc in npcs)
        {
            if (npc.TryGetComponent<NPCController>(out var npcController))
            {   
                if(npcController.alive)
                {
                    NPCData data = new(npcController.speed, npcController.strength);
                    // Debug.Log("NPC speed:" + data.speed + "  strength:" + data.strength);
                    npcDataList.Add(data);
                }
            }
        }

        string json = JsonUtility.ToJson(new NPCDataList { npcs = npcDataList }, true);
        File.WriteAllText(Path.Combine(Application.persistentDataPath, fileName), json);
        Debug.Log("NPC data saved to " + Application.persistentDataPath + "/" + fileName);
    }

    /// <summary>
    /// load NPC data from file
    /// </summary>
    static public List<NPCData> LoadNPCs(string fileName = "npcData.json")
    {

        string filePath = Path.Combine(Application.persistentDataPath, fileName);
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            NPCDataList npcDataList = JsonUtility.FromJson<NPCDataList>(json);

            Debug.Log("NPC data loaded from " + filePath);
            return npcDataList.npcs;
        }
        else
        {
            Debug.LogWarning("No NPC data file found at " + filePath);
            return new List<NPCData>();
        }
    }

    public static Vector3 GenerateRandomPosition(float min, float max)
    {
        float x = Random.Range(min, max);
        return new Vector3(x, 0, 0);
    }

    public static List<NPCData> NextGeneration(List<NPCData> oldGeneration)
    {
        List<NPCData> nextGeneration = new();

        GA ga = new();
        float[,] parents = new float[oldGeneration.Count, 2];
        for (int i = 0; i < oldGeneration.Count; i++)
        {
            float[] dna = oldGeneration[i].GetDNA();
            for (int j = 0; j < 2; j++) parents[i, j] = dna[j];
        }

        float[,] generations = ga.GetGenerations(parents, 2);
        for (int i = 0; i < generations.GetLength(0); i++)
        {
            float[] dna = new float[generations.GetLength(1)]; 
            for (int j = 0; j < generations.GetLength(1); j++) dna[j] = generations[i, j];
            NPCData npc = new();
            npc.UseDNA(dna);
            nextGeneration.Add(npc);
        }

        return nextGeneration;
    }

}

