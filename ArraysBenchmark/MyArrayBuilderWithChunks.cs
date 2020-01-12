using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp1
{
    public class MyArrayBuilderWithChunks
    {
        private const int DefaultCapacity = 4;

        public T[] GetArray<T>(IEnumerable<T> enumerable)
        {
            using var enumerator = enumerable.GetEnumerator();
            var arrays = new T[][] { };
            var current = new T[] { };
            var arraysCapacity = 0;
            var elemsCapacity = 0;
            var elemsSize = 0;
            var arraysSize = 0;
            var length = 0;

            while (enumerator.MoveNext())
            {
                if ((uint) elemsSize < (uint) elemsCapacity)
                {
                    current[elemsSize] = enumerator.Current;
                    elemsSize++;
                }
                else
                {
                    if ((uint) arraysSize < (uint) arraysCapacity)
                    {
                        arrays[arraysSize] = current;
                        arraysSize++;
                    }
                    else
                    {
                        arraysCapacity = arraysCapacity == 0 ? DefaultCapacity : arraysCapacity * 2;
                        var newArrays = new T[arraysCapacity][];
                        if (arraysSize > 0)
                        {
                            Array.Copy(arrays, 0, newArrays, 0, arraysSize);
                        }

                        newArrays[arraysSize] = current;
                        arraysSize++;
                        arrays = newArrays;
                    }

                    elemsCapacity = elemsCapacity == 0 ? DefaultCapacity : elemsCapacity * 2;
                    current = new T[elemsCapacity];
                    current[0] = enumerator.Current;
                    elemsSize = 1;
                }

                length++;
            }

            var result = new T[length];
            var copyIndex = 0;
            for (var c = 0; c < arraysSize; c++)
            {
                var arr = arrays[c];
                Array.Copy(arr, 0, result, copyIndex, arr.Length);
                copyIndex += arr.Length;
            }

            if (current.Length != 0)
            {
                Array.Copy(current, 0, result, copyIndex, Math.Min(length - copyIndex, current.Length));
            }

            return result;
        }
    }
}