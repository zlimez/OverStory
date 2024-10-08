using UnityEngine;

namespace Abyss.Environment.Enemy
{
    public class HogSpawner : EnemySpawner
    {
        [SerializeField] Transform[] waypoints;

        public override bool Spawn(object attr)
        {
            if (!base.Spawn(attr)) return false;
            HogBT hogBT = _instance.GetComponent<HogBT>();
            hogBT.waypoints = waypoints;
            hogBT.Setup();
            return true;
        }
    }
}
