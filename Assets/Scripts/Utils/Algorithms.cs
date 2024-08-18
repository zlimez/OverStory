using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Algorithms
{
    public static class Search {
        public static int BinarySearchClosest<T>(this List<T> list, T target) where T : IComparable<T>
        {
            int min = 0;
            int max = list.Count - 1;

            while (min <= max)
            {
                int mid = (min + max) / 2;
                int comparisonResult = target.CompareTo(list[mid]);

                if (comparisonResult == 0)
                {
                    // target element found
                    return mid;
                }
                else if (comparisonResult < 0)
                {
                    // target element is less than mid element
                    max = mid - 1;
                }
                else
                {
                    // target element is greater than mid element
                    min = mid + 1;
                }
            }

            // target element not found, return index of closest match
            if (max < 0)
            {
                // target element is less than all elements in the list
                return 0;
            }
            else if (min >= list.Count)
            {
                // target element is greater than all elements in the list
                return list.Count - 1;
            }
            else
            {
                // return index of closest element
                return Math.Abs(target.CompareTo(list[min])) < Math.Abs(target.CompareTo(list[max])) ? min : max;
            }
        }
    }

    public static class Tools {
        public static List<T> Merge<T>(List<T> sortedList1, List<T> sortedList2) where T : IComparable<T> {
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
