using System.Collections.Generic;
using UnityEngine;
using Tuples;
using Abyss.EventSystem;

[CreateAssetMenu(menuName = "Event Set")]
public class EventSetAsset : ScriptableObject
{
    [SerializeField]
    public List<Pair<GameEvent, int>> LoopedPastEvents;

    [SerializeField]
    public List<Pair<GameEvent, int>> NormalPastEvents;
}
