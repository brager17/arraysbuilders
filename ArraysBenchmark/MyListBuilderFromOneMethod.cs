using System;
using System.Collections.Generic;

namespace ConsoleApp1
{
    public class MyArrayBuilderFromOneMethod
    {
        public T[] GetArray<T>(IEnumerable<T> enumerable)
        {
            using (var enumerator = enumerable.GetEnumerator())
            {
                var arr = new T[4];
                var capacity = 4;
                var size = 0;
                while (enumerator.MoveNext())
                {
                    if ((uint) size < (uint) capacity)
                    {
                        arr[size] = enumerator.Current;
                        size++;
                    }
                    else
                    {
                        capacity *= 2;
                        var newArr = new T[capacity];
                        Array.Copy(arr, 0, newArr, 0, size);
                        newArr[size] = enumerator.Current;
                        arr = newArr;
                        size++;
                    }
                }

                return arr;
            }
        }
    }
}