using System.Collections.Generic;
using UnityEngine;
using Tuples;
using Chronellium.EventSystem;

[CreateAssetMenu(fileName = "EventSet", menuName = "Chronellium/EventSet")]
public class EventSetAsset : ScriptableObject
{
    [SerializeField]
    public List<Pair<GameEvent, int>> LoopedPastEvents;

    [SerializeField]
    public List<Pair<GameEvent, int>> NormalPastEvents;
}
