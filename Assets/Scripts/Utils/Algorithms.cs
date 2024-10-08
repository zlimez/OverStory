using System.Collections.Generic;
using System;
using System.Linq;

namespace Algorithms
{
    public static class Search
    {
        public static int BinFindLEQ<T>(this List<T> list, T target, int l = 0, int r = -1) where T : IComparable<T>
        {
            r = r == -1 ? list.Count - 1 : r;

            while (l <= r)
            {
                int mid = (l + r) / 2;
                int comparisonResult = target.CompareTo(list[mid]);

                if (comparisonResult == 0)
                    return mid;
                else if (comparisonResult < 0)
                    r = mid - 1;
                else l = mid + 1;
            }

            return r;
        }

        public static int BinFindClosest<T>(this List<T> list, T target) where T : IComparable<T>
        {
            int l = 0;
            int r = list.Count - 1;

            while (l <= r)
            {
                int mid = (l + r) / 2;
                int comparisonResult = target.CompareTo(list[mid]);

                if (comparisonResult == 0)
                    return mid;
                else if (comparisonResult < 0)
                    r = mid - 1;
                else l = mid + 1;
            }

            if (r < 0)
                return 0;
            else if (l >= list.Count)
                return list.Count - 1;
            else return Math.Abs(target.CompareTo(list[l])) < Math.Abs(target.CompareTo(list[r])) ? l : r;
        }
    }

    public static class ListUtils
    {
        // Not very efficient, but good enough for small lists
        public static List<int> WeightedShuffle<T>(List<float> probDist, List<T> target)
        {
            float sum = probDist.Sum();
            for (int i = 0; i < probDist.Count; i++) probDist[i] /= sum;
            sum = 1f;
            List<int> order = new();
            for (int i = 0; i < target.Count; i++) order.Add(i);

            var ran = new Random();
            List<float> accProbDist = new();
            for (int i = 0; i < target.Count - 1; i++)
            {
                float rval = (float)ran.NextDouble() * sum;
                for (int j = i; j < probDist.Count; j++) accProbDist.Add(j == i ? 0 : accProbDist[j - i - 1] + probDist[j - 1]);
                int ind = Search.BinFindLEQ(accProbDist, rval, i);
                sum -= probDist[ind];
                accProbDist.Clear();
                if (ind == i) continue;
                (target[ind], target[i]) = (target[i], target[ind]);
                (order[ind], order[i]) = (order[i], order[ind]);
                (probDist[ind], probDist[i]) = (probDist[i], probDist[ind]);
            }
            return order;
        }

        public static void ShuffleForK<T>(int k, List<T> target)
        {
            Random ran = new();
            for (int i = 0; i < k; i++)
            {
                int j = ran.Next(i, target.Count);
                (target[i], target[j]) = (target[j], target[i]);
            }
        }

        public static List<T> Merge<T>(List<T> sortedList1, List<T> sortedList2) where T : IComparable<T>
        {
            List<T> mergedList = new List<T>();
            int index1 = 0;
            int index2 = 0;

            while (index1 < sortedList1.Count && index2 < sortedList2.Count)
            {
                if (sortedList1[index1].CompareTo(sortedList2[index2]) < 0)
                {
                    mergedList.Add(sortedList1[index1]);
                    index1++;
                }
                else
                {
                    mergedList.Add(sortedList2[index2]);
                    index2++;
                }
            }

            while (index1 < sortedList1.Count)
            {
                mergedList.Add(sortedList1[index1]);
                index1++;
            }

            while (index2 < sortedList2.Count)
            {
                mergedList.Add(sortedList2[index2]);
                index2++;
            }

            return mergedList;
        }
    }
}
