using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathLibrary
{
    public static class Sort
    {
        public delegate bool Compare<T>(T x, T y);

        public static void QuickSort<T>(T[] array, int beginIndex, int endIndex, Compare<T> compare)
        {
            int i = beginIndex;
            int j = endIndex;
            T mid = array[(endIndex + beginIndex) / 2];
            do
            {
                while (compare(array[i], mid))
                    ++i;
                while (compare(mid, array[j]))
                    --j;
                if (i <= j)
                {
                    UsefulStuff.Swap(ref array[i], ref array[j]);
                    ++i;
                    --j;
                }
            } while (i <= j);
            if (j > beginIndex)
                QuickSort(array, beginIndex, j, compare);
            if (i < endIndex)
                QuickSort(array, i, endIndex, compare);
        }

        public static void BubbleSort<T>(T[] array, int beginIndex, int endIndex, Compare<T> compare)
        {
            for (int i = beginIndex; i < endIndex; ++i)
                for (int j = beginIndex; j < endIndex - i; ++j)
                    if (compare(array[j + 1], array[j]))
                        UsefulStuff.Swap(ref array[j + 1], ref array[j]);
        }

        public static void InsertionSort<T>(T[] array, int beginIndex, int endIndex, Compare<T> compare)
        {
            for (int i = beginIndex + 1; i <= endIndex; ++i)
                for (int j = i; j > beginIndex && compare(array[j], array[j - 1]); --j)
                    UsefulStuff.Swap(ref array[j], ref array[j - 1]);
        }

        private static void Merge<T>(T[] arr, int l, int m, int r, Compare<T> compare)
        {
            int i, j, k;
            int n1 = m - l + 1;
            int n2 = r - m;

            T[] L = new T[n1], R = new T[n2];

            for (i = 0; i < n1; ++i)
                L[i] = arr[l + i];
            for (j = 0; j < n2; ++j)
                R[j] = arr[m + 1 + j];

            i = 0;
            j = 0; 
            k = l;
            while (i < n1 && j < n2)
            {
                if (compare(L[i], R[j]))
                {
                    arr[k] = L[i];
                    ++i;
                }
                else
                {
                    arr[k] = R[j];
                    ++j;
                }
                ++k;
            }

            while (i < n1)
            {
                arr[k] = L[i];
                ++i;
                ++k;
            }

            while (j < n2)
            {
                arr[k] = R[j];
                ++j;
                ++k;
            }
        }

        public static void MergeSort<T>(T[] arr, int beginIndex, int endIndex, Compare<T> compare)
        {
            if (beginIndex < endIndex)
            {
                int m = beginIndex + (endIndex - beginIndex) / 2;

                MergeSort(arr, beginIndex, m, compare);
                MergeSort(arr, m + 1, endIndex, compare);

                Merge(arr, beginIndex, m, endIndex, compare);
            }
        }

        public static void SelectionSort<T>(T[] arr, int beginIndex, int endIndex, Compare<T> compare)
        {
            for (int i = beginIndex; i < endIndex; ++i)
            {
                int minIndex = i;
                for (int j = i + 1; j <= endIndex; ++j)
                    if (compare(arr[j], arr[minIndex]))
                        minIndex = j;

                UsefulStuff.Swap(ref arr[minIndex], ref arr[i]);
            }
        }

        public static void CountingSort(int[] arr, int beginIndex, int endIndex)
        {
            int max = arr.Max();
            int n = endIndex - beginIndex + 1;
            int[] ret = new int[n + 1];
            int[] c = new int[max + 1];
            for (int i = beginIndex; i <= endIndex; ++i)
                ++c[arr[i]];
            for (int i = 1; i < max + 1; ++i)
                c[i] += c[i - 1];

            for (int i = endIndex; i >= beginIndex; --i)
            {
                if (c[arr[i]] > 0)
                {
                    ret[c[arr[i]]] = arr[i];
                    --c[arr[i]];
                }
            }

            int j = beginIndex + 1;
            for (int i = beginIndex; i <= endIndex; ++i)
                arr[i] = ret[j++];
        }
    }
}
