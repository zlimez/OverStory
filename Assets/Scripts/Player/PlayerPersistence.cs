using System;
using Abyss.Player;

namespace Abyss.DataPersistence
{
    [Serializable]
    public class PlayerPersistence
    {
        public PlayerAttr PlayerAttr = new(1, 1, 1, 100, 80);
        public WeaponItem WeaponItem;
        public int InventoryLevel = 1;
    }

    [Serializable]
    public class TimePersistence
    {
        public float TimeOfCycle = 0;
        public float TtTime = 0;
    }
}
