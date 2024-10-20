using System;
using Abyss.Player;
using Abyss.SceneSystem;
using Tuples;

namespace Abyss.DataPersistence
{
    [Serializable]
    public class PlayerPersistence
    {
        public PlayerAttr PlayerAttr = new(1, 1, 1, 100, 80);
        public WeaponItem WeaponItem;
        public Pair<AbyssScene, UnityEngine.Vector3> LastRest;
        public bool JustDied = false;
    }

    [Serializable]
    public class TimePersistence
    {
        public float TimeOfCycle = 0;
        public float TtTime = 0;
    }
}
