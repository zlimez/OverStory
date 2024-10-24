using System;

namespace Tuples
{
    [Serializable]
    public struct Pair<U, T>
    {
        public Pair(U head, T tail)
        {
            Head = head;
            Tail = tail;
        }
        public U Head;
        public T Tail;
    }

    [Serializable]
    public struct Triplet<U, T, Z>
    {
        public U Item1;
        public T Item2;
        public Z Item3;
        public Triplet(U item1, T item2, Z item3)
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
        }
    }

    [Serializable]
    public class RefPair<U, T>
    {
        public RefPair(U head, T tail)
        {
            Head = head;
            Tail = tail;
        }
        public U Head;
        public T Tail;
    }

    [Serializable]
    public class RefTriplet<U, T, Z>
    {
        public U Item1;
        public T Item2;
        public Z Item3;
        public RefTriplet(U item1, T item2, Z item3)
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
        }
    }
}
