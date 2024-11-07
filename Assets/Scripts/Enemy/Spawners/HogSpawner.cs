using UnityEngine;

namespace Abyss.Environment.Enemy
{
    public class HogSpawner : EnemySpawner
    {
        [SerializeField] Transform[] waypoints;
        [SerializeField] Arena arena;

        public override bool Spawn(object attr, Transform parent)
        {
            if (!base.Spawn(attr, parent)) return false;
            HogBT hogBT = _instance.GetComponent<HogBT>();
            hogBT.Waypoints = waypoints;
            hogBT.Arena = arena;
            hogBT.Setup();
            return true;
        }
    }
}
