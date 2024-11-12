using UnityEngine;
using UnityEngine.Assertions;

namespace Abyss.Environment.Enemy
{
    public class HogSpawner : EnemySpawner
    {
        [SerializeField] Transform[] waypoints;
        [SerializeField] Transform patrolLeft, patrolRight;
        [SerializeField] Arena arena;

        public override bool Spawn(object attr, Transform parent)
        {
            if (!base.Spawn(attr, parent)) return false;
            HogBT hogBT = _instance.GetComponent<HogBT>();
            hogBT.SpecyName = specy.specyName;
            hogBT.Waypoints = waypoints;
            hogBT.PatrolLeft = patrolLeft;
            hogBT.PatrolRight = patrolRight;
            hogBT.Arena = arena;
#if UNITY_EDITOR
            Assert.IsTrue(hogBT.Arena.Movable, "Hog arena should be movable");
#endif
            hogBT.Setup();
            return true;
        }
    }
}
