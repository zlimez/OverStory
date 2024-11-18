using Tuples;
using UnityEngine;

[CreateAssetMenu(menuName = "Specy Attributes")]
public class SpecyAttr : ScriptableObject
{
    public string specyName;
    public float mutationRate;
    public int maxPopulation;
    public float generationRate;
    [Tooltip("Base strength exp given to player when defeated")] public float baseStrExpGiven;

    [Tooltip("Attr name, min and max")]
    public Triplet<string, float, float>[] attrRanges;

    public Pair<float, float>[] AllAttrRanges()
    {
        Pair<float, float>[] allAttrRanges = new Pair<float, float>[attrRanges.Length];
        for (int i = 0; i < allAttrRanges.Length; i++)
            allAttrRanges[i] = new Pair<float, float>(attrRanges[i].Item2, attrRanges[i].Item3);

        return allAttrRanges;
    }
}
