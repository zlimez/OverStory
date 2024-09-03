using UnityEngine;

// [CreateAssetMenu(fileName = "NPC", menuName = "ScriptableObjects/NPC", order = 1)]


[System.Serializable]
public class NPCData
{
    // public bool alive;
    public float speed;
    public float strength;

     public NPCData()
    {
        
        // speed = s1;
        // strength = s2;
    }
    public NPCData(float s1, float s2)
    {
        speed = s1;
        strength = s2;
    }

    public float[] GetDNA()
    {
        float[] dna = new float[] {speed, strength};
        return dna;
    }
    public void UseDNA(float[] dna)
    {
        speed = dna[0];
        strength = dna[1];
    }

}
