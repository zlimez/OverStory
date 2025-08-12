using UnityEngine;

namespace VerletPhysics
{
    struct Point
    {
        public Vector2 Pos;
        public Vector2 OldPos;
        public bool Pinned;
    }

    struct Stick
    {
        public int P0;
        public int P1;
        public float Length;
        public bool MaxOnly; // Enforces distance only if geq Length
    }
}
