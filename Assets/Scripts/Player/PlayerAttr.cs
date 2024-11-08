using System;

namespace Abyss.Player
{
    [Serializable]
    public class PlayerAttr
    {
        public static readonly float MaxStrength = 50;
        public static readonly float MaxIntelligence = 50;
        public static readonly float MaxAgility = 50;
        public static readonly float MaxHealth = 100;
        public static readonly float MaxPurity = 100;
        public static readonly float MaxActionPurity = 100;
        public static readonly float MaxFriendlinessPurity = 100;

        public float Strength;
        public float Intelligence;
        public float Agility;
        public float Health;
        public float Purity;
        public float ActionPurity;
        public float FriendlinessPurity;

        public PlayerAttr(float strength, float intelligence, float agility, float health, float actionPurity, float friendlinessPurity)
        {
            Strength = strength;
            Intelligence = intelligence;
            Agility = agility;
            Health = health;
            ActionPurity = actionPurity;
            FriendlinessPurity = friendlinessPurity;
            Purity = actionPurity + friendlinessPurity;
        }

        public bool IsLessThanOrEqual(PlayerAttr other)
        {
            return Strength <= other.Strength &&
                Intelligence <= other.Intelligence &&
                Agility <= other.Agility &&
                // Health <= other.Health &&
                Purity <= other.Purity;
        }
    }
}
