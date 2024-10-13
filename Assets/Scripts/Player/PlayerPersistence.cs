using System;

namespace Abyss.Player
{
    [Serializable]
    public class PlayerPersistence
    {
        public PlayerAttr PlayerAttr = new(1, 1, 1, 100, 65);
        public WeaponItem WeaponItem;
        public int InventoryLevel = 1;
    }
}
