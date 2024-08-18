using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

// FIXME: Possible issue with implementation? 
namespace DataStructures {
    public class MyQueue<T>
    {
        private List<T> queue = new List<T>();
        private int batchRemovalSize;
        private int frontPointer = 0;
        private int backPointer = 0;

        public MyQueue(int batchRemovalSize = 32) {
            this.batchRemovalSize = batchRemovalSize;
        }

        public void Enqueue(T item) {
            queue.Add(item);
            backPointer += 1;
        }

        public T Dequeue() {
            if (Count() == 0) {
                return default(T);
            }

            T item = queue[frontPointer];
            frontPointer += 1;
            if (frontPointer == batchRemovalSize) {
                queue.RemoveRange(0, batchRemovalSize);
                frontPointer = 0;
                backPointer -= batchRemovalSize;
            }
            return item;
        }

        public T Peek() {
            return queue[frontPointer];
        }

        public int Count() {
            return backPointer - frontPointer;
        }

        // Removes last occurrence of item from queue
        public void Remove(T item) {
            int lastOccurrence = queue.FindLastIndex(x => x.Equals(item));
            if (lastOccurrence < frontPointer) return;
            queue.RemoveAt(lastOccurrence);
            backPointer -= 1;
        }

        public void RemoveAll() {
            queue.Clear();
        }

        public bool Contains(T item) {
            return queue.FindIndex(x => x.Equals(item)) >= frontPointer;
        }

        public T Last() {
            if (Count() == 0) {
                return default(T);
            }

            return queue[backPointer - 1];
        }
    }

    // Doubly linked circular linked list
    public class CircularLinkedList<T> {
        private LinkedNode<T> currentNode = null;
        
        public void Add(T item) {
            if (currentNode == null) {
                currentNode = new LinkedNode<T>(item);
                currentNode.Next = currentNode;
                currentNode.Previous = currentNode;
            } else {
                var addedNode = new LinkedNode<T>(item);
                addedNode.Next = currentNode.Next;
                addedNode.Previous = currentNode;
                currentNode.Next.Previous = addedNode;
                currentNode.Next = addedNode;
                currentNode = addedNode;
            }
        }

        // After removal, current node becomes the next node in line
        public void Remove() {
            LinkedNode<T> removedNode = currentNode;
            currentNode = currentNode.Next;
            currentNode.Previous = removedNode.Previous;
            currentNode.Previous.Next = currentNode;
            removedNode.Previous = null;
            removedNode.Next = null;
        }

        public LinkedNode<T> Next => currentNode.Next;
        public LinkedNode<T> Previous => currentNode.Previous;
        public LinkedNode<T> Current => currentNode;

        public LinkedNode<T> GoNext() {
            currentNode = Next;
            return currentNode;
        }

        public LinkedNode<T> GoPrevious() {
            currentNode = Previous;
            return currentNode;
        }
    }

    public class LinkedNode<T> {
        private T item;
        public LinkedNode<T> Previous { get; set; }
        public LinkedNode<T> Next { get; set; }

        public LinkedNode(T item) {
            this.item = item;
        }
    }

    // Upon initialized items can neither be added or removed
    public class CircularList<T> {
        public List<T> Items { get; private set; }
        private int pointer = 0;

        public CircularList(List<T> items) {
            this.Items = items;
        }

        public T Current => Items[pointer];
        public int Count => Items.Count;

        public T Next() {
            pointer++;
            pointer = pointer >= Items.Count ? pointer - Items.Count : pointer;
            return Items[pointer];
        }

        public T Previous() {
            pointer--;
            pointer = pointer < 0 ? Items.Count - 1 : pointer;
            return Items[pointer];
        }

        public T Get(int index) {
            index = index % Items.Count;
            return Items[index];
        }
    }

    public class PriorityQueue<T>
    {
        private List<T> heap;
        private IComparer<T> comparer;

        public PriorityQueue() : this(Comparer<T>.Default)
        {
        }

        public PriorityQueue(IComparer<T> comparer)
        {
            this.heap = new List<T>();
            this.comparer = comparer;
        }

        public void Enqueue(T item)
        {
            this.heap.Add(item);
            int i = this.heap.Count - 1;

            while (i > 0)
            {
                int parentIndex = (i - 1) / 2;

                if (this.comparer.Compare(this.heap[parentIndex], this.heap[i]) <= 0)
                {
                    break;
                }

                T temp = this.heap[i];
                this.heap[i] = this.heap[parentIndex];
                this.heap[parentIndex] = temp;
                i = parentIndex;
            }
        }

        public T Dequeue()
        {
            int lastIndex = this.heap.Count - 1;
            T frontItem = this.heap[0];
            this.heap[0] = this.heap[lastIndex];
            this.heap.RemoveAt(lastIndex);
            lastIndex--;

            int i = 0;

            while (true)
            {
                int leftChild = i * 2 + 1;
                int rightChild = i * 2 + 2;
                int smallestChild = i;

                if (leftChild <= lastIndex && this.comparer.Compare(this.heap[leftChild], this.heap[smallestChild]) < 0)
                {
                    smallestChild = leftChild;
                }

                if (rightChild <= lastIndex && this.comparer.Compare(this.heap[rightChild], this.heap[smallestChild]) < 0)
                {
                    smallestChild = rightChild;
                }

                if (smallestChild == i)
                {
                    break;
                }

                T temp = this.heap[i];
                this.heap[i] = this.heap[smallestChild];
                this.heap[smallestChild] = temp;
                i = smallestChild;
            }

            return frontItem;
        }

        public T Peek()
        {
            if (this.heap.Count == 0)
            {
                throw new InvalidOperationException("The priority queue is empty.");
            }

            return this.heap[0];
        }


        public int Count
        {
            get { return this.heap.Count; }
        }

        public bool IsEmpty => Count == 0;
    }
}
