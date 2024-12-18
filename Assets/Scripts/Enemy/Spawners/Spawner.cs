using UnityEngine;

namespace Abyss.Environment
{
    public class Spawner : MonoBehaviour
    {
        public GameObject entityPrefab;
        public Transform spawnPoint;
        protected GameObject _instance;

        // Passes any required auxiliary info to the instance via fields and attr
        // Invoked by SpawnerManager of scene during Start
        public virtual bool Spawn(object attr, Transform parent)
        {
            if (attr == null)
            {
                _instance = null;
                return false;
            }
            if (_instance != null) Destroy(_instance);
            _instance = Instantiate(entityPrefab, spawnPoint.position, Quaternion.identity);
            _instance.transform.SetParent(parent);
            return true;
        }
    }
}
