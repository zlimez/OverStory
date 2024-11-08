using System;
using Abyss.Player;
using Abyss.SceneSystem;
using Tuples;

namespace Abyss.DataPersistence
{
    [Serializable]
    public class PlayerPersistence
    {
        public PlayerAttr PlayerAttr = new(1, 1, 1, PlayerAttr.MaxHealth, 60, 20);
        public WeaponItem WeaponItem;
        public SpellItem[] SpellItems = new SpellItem[3];
        public Pair<AbyssScene, UnityEngine.Vector3> LastRest;
        public bool JustDied = false;
        public int DroneLevel = 1;
    }

    [Serializable]
    public class TimePersistence
    {
        public float TimeOfCycle = 0;
        public float TotalTime = 0;
    }
}
