using System.Collections.Generic;
using Abyss.Environment.Enemy;
using Abyss.EventSystem;
using UnityEngine;

namespace Abyss.Environment
{
    public class SpawnerManager : MonoBehaviour
    {
        public Spawner[] spawners;
        Dictionary<string, List<Spawner>> enemyTypes = new();

        void Start()
        {
            foreach (var spawner in spawners)
            {
                if (spawner.entityPrefab.TryGetComponent(out EnemyManager enemy))
                {
                    if (!enemyTypes.ContainsKey(enemy.specyAttr.specyName)) enemyTypes.Add(enemy.specyAttr.specyName, new());
                    enemyTypes[enemy.specyAttr.specyName].Add(spawner);
                }
            }

            if (EnemyPopManager.Instance == null || !EnemyPopManager.Instance.IsReady)
                EventManager.StartListening(EnemyPopManager.EnemyPopManagerReady, Setup);
            else Setup();
        }

        void OnDisable()
        {
            EventManager.StopListening(EnemyPopManager.EnemyPopManagerReady, Setup);
        }

        void Setup(object input = null)
        {
            foreach (var enemyType in enemyTypes)
            {
                var instancesAttr = EnemyPopManager.Instance.GetSpecyInstances(enemyType.Key, enemyType.Value.Count);
                for (int i = 0; i < enemyType.Value.Count; i++)
                    enemyType.Value[i].Spawn(instancesAttr[i]);
            }
        }
    }
}
