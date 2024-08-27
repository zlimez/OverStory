using System.Collections.Generic;
using UnityEngine;
using Tuples;
using Abyss.EventSystem;

[CreateAssetMenu(fileName = "EventSet", menuName = "Abyss/EventSet")]
public class EventSetAsset : ScriptableObject
{
    [SerializeField]
    public List<Pair<GameEvent, int>> LoopedPastEvents;

    [SerializeField]
    public List<Pair<GameEvent, int>> NormalPastEvents;
}
