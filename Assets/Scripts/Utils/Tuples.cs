using System;

namespace Tuples {
    [Serializable]
    public struct Pair<U, T> {
        public Pair(U head, T tail) {
            this.Head = head;
            this.Tail = tail;
        }
        public U Head;
        public T Tail;
    }

    [Serializable]
    public struct Triplet<U, T, Z> {
        public U Item1;
        public T Item2;
        public Z Item3;
         public Triplet(U item1, T item2, Z item3) {
            this.Item1 = item1;
            this.Item2 = item2;
            this.Item3 = item3;
        }
    }
}
