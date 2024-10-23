using UnityEngine;

namespace Abyss.Environment.Enemy
{
    public class EnemySpawner : Spawner
    {
        public override bool Spawn(object attr, Transform parent)
        {
            if (!base.Spawn(attr, parent)) return false;
            EnemyAttr providedAttr = (EnemyAttr)attr;
            _instance.GetComponent<EnemyManager>().attributes = providedAttr;
            return true;
        }
    }
}
