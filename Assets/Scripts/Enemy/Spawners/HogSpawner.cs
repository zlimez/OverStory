using UnityEngine;

namespace Environment.Enemy
{
    public class HogSpawner : EnemySpawner
    {
        [SerializeField] Transform[] waypoints;
        [SerializeField] Arena arena;

        public override bool Setup(object attr)
        {
            if (!base.Setup(attr)) return false;
            HogBT hogBT = _instance.GetComponent<HogBT>();
            hogBT.waypoints = waypoints;
            hogBT.arena = arena;
            hogBT.Setup();
            return true;
        }
    }
}
