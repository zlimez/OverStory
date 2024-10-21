using UnityEngine;

namespace Abyss.Environment.Enemy
{
    public class BugSpawner : EnemySpawner
    {
        [SerializeField] Arena arena;
        [SerializeField] Transform leftEnd;
        [SerializeField] Transform rightEnd;

        public override bool Spawn(object attr)
        {
            if (!base.Spawn(attr)) return false;
            BugBT bugBT = _instance.GetComponent<BugBT>();
            bugBT.Arena = arena;
            bugBT.LeftEnd = leftEnd;
            bugBT.RightEnd = rightEnd;
            bugBT.Setup();
            return true;
        }
    }
}