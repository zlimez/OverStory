using UnityEngine;

namespace VerletPhysics
{
    internal struct Point
    {
        public Vector2 Pos;
        public Vector2 OldPos;
        public bool Pinned;
        public float Mass;
    }

    internal struct Stick
    {
        public int P0;
        public int P1;
        public float Length;
        public bool MaxOnly; // Enforces distance only if geq Length
    }
}
