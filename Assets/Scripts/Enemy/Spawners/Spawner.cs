using UnityEngine;

namespace Abyss.Environment
{
    public class Spawner : MonoBehaviour
    {
        public GameObject entityPrefab;
        protected GameObject _instance;

        // Passes any required auxiliary info to the instance via fields and attr
        // Invoked by SpawnerManager of scene during Start
        public virtual bool Spawn(object attr)
        {
            if (attr == null)
            {
                _instance = null;
                return false;
            }
            _instance = Instantiate(entityPrefab, transform.position, Quaternion.identity);
            return true;
        }
    }
}
