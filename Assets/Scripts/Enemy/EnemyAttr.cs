using System;

namespace Abyss.Environment.Enemy
{
    [Serializable]
    public class EnemyAttr
    {
        public bool isAlive = true;
        public float strength;
        public float speed;
        public float alertness;
        public float friendliness;

        public float[] DNA => new float[] { strength, speed, alertness, friendliness };

        public void UseDNA(float[] dna)
        {
            strength = dna[0];
            speed = dna[1];
            alertness = dna[2];
            friendliness = dna[3];
        }
    }
}