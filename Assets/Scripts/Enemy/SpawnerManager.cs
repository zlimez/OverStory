using System.Collections.Generic;
using Abyss.Environment.Enemy;
using Abyss.EventSystem;
using UnityEngine;

namespace Abyss.Environment
{
    public class SpawnerManager : MonoBehaviour
    {
        public Spawner[] spawners;
        [SerializeField] Transform instancesParent;
        readonly Dictionary<string, List<Spawner>> enemyTypes = new();

        void Awake()
        {
            foreach (var spawner in spawners)
            {
                if (spawner is EnemySpawner enemy)
                {
                    if (!enemyTypes.ContainsKey(enemy.Specy.specyName)) enemyTypes.Add(enemy.Specy.specyName, new());
                    enemyTypes[enemy.Specy.specyName].Add(spawner);
                }
            }
        }

        void OnEnable()
        {
            if (EnemyPopManager.Instance == null || !EnemyPopManager.Instance.IsReady)
                EventManager.StartListening(SystemEvents.EnemyPopManagerReady, Setup);
            else Setup();
            EventManager.StartListening(PlayEvents.RestEnd, Setup);
        }

        void OnDisable()
        {
            EventManager.StopListening(PlayEvents.RestEnd, Setup);
            EventManager.StopListening(SystemEvents.EnemyPopManagerReady, Setup);
        }

        void Setup(object input = null)
        {
            foreach (var enemyType in enemyTypes)
            {
                var instancesAttr = EnemyPopManager.Instance.GetSpecyInstances(enemyType.Key, enemyType.Value.Count);
                if (instancesAttr == null) continue;
                for (int i = 0; i < enemyType.Value.Count; i++)
                    enemyType.Value[i].Spawn(instancesAttr[i], instancesParent);
            }
        }
    }
}
