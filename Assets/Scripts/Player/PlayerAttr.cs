using System;

namespace Abyss.Player
{
    [Serializable]
    public struct PlayerAttr
    {
        public static readonly float MaxStrength = 50;
        public static readonly float MaxIntelligence = 50;
        public static readonly float MaxAgility = 50;
        public static readonly float MaxHealth = 100;
        public static readonly float MaxPurity = 100;

        public float Strength;
        public float Intelligence;
        public float Agility;
        public float Health;
        public float Purity;

        public PlayerAttr(float strength, float intelligence, float agility, float health, float purity)
        {
            this.Strength = strength;
            this.Intelligence = intelligence;
            this.Agility = agility;
            this.Health = health;
            this.Purity = purity;
        }
    }
}
