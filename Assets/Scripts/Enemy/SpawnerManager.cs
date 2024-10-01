using System.Collections;
using System.Collections.Generic;
using Abyss.Environment;
using Abyss.Environment.Enemy;
using UnityEngine;

namespace Abyss.Environment
{
    public class SpawnerManager : MonoBehaviour
    {
        public Spawner[] spawners;

        void Start()
        {
            Dictionary<string, List<Spawner>> enemyTypes = new();
            foreach (var spawner in spawners)
            {
                if (spawner.entityPrefab.TryGetComponent<EnemyManager>(out EnemyManager enemy))
                {
                    if (!enemyTypes.ContainsKey(enemy.specyAttr.specyName)) enemyTypes.Add(enemy.specyAttr.specyName, new());
                    enemyTypes[enemy.specyAttr.specyName].Add(spawner);
                }
            }

            foreach (var enemyType in enemyTypes)
            {
                var instancesAttr = EnemyPopManager.Instance.GetSpecyInstances(enemyType.Key, enemyType.Value.Count);
                for (int i = 0; i < enemyType.Value.Count; i++)
                    enemyType.Value[i].Setup(instancesAttr[i]);
            }
        }
    }
}
