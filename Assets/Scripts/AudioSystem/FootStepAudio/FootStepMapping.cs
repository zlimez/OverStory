using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Footstep Mapping")]
public class FootStepMapping : ScriptableObject
{
    public List<Material> materials;
    public List<AudioClip> sounds;
    public List<string> GetMaterialNames()
    {
        List<string> materialNames = new List<string>();

        foreach (Material mat in materials)
        {
            materialNames.Add(mat.name);
        }

        return materialNames;
    }

    public virtual AudioClip GetRandomSound()
    {
        return sounds[Random.Range(0, sounds.Count)];
    }
}
