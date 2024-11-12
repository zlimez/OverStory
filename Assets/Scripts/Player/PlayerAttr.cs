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

        public enum ExpAttrType
        {
            Strength,
            Agility
        }

        float _expBaseReq = 10f;
        float _expReqMultiplier = 1.5f;
        float _strExpReq => _expBaseReq * (float)Math.Pow(_expReqMultiplier, Strength - 1);
        float _agiExpReq => _expBaseReq * (float)Math.Pow(_expReqMultiplier, Agility - 1);
        float _strLvlExp = 0, _agiLvlExp = 0;

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

        public void FeedExp(ExpAttrType expAttrType, float exp)
        {
            switch (expAttrType)
            {
                case ExpAttrType.Strength:
                    _strLvlExp += exp;
                    if (_strLvlExp >= _strExpReq)
                    {
                        _strLvlExp -= _strExpReq;
                        Strength++;
                    }
                    break;
                case ExpAttrType.Agility:
                    _agiLvlExp += exp;
                    if (_agiLvlExp >= _agiExpReq)
                    {
                        _agiLvlExp -= _agiExpReq;
                        Agility++;
                    }
                    break;
            }
        }
    }
}
