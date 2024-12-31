using Abyss.EventSystem;
using UnityEngine;

namespace NPC
{
    public abstract class ActionNode : ScriptableObject
    {
        public ActionNode Next;
        public abstract void Execute(GameEvent interactEvent);
    }
}
