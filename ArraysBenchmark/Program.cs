using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using FluentAssertions;

namespace ConsoleApp1
{
    public struct PersonStruct
    {
        public string Name;
        public byte Age;
    }

    public class PersonClass
    {
        public string Name;
        public byte Age;
    }

    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<CompareArraysBuildersBenchmark>();
        }
    }

    [MemoryDiagnoser]
    public class CompareArraysBuildersBenchmark
    {
        private ArrayBuilderAsList<PersonStruct> _asList;
        private ArrayBuilderWithChunksUsage<PersonStruct> _useChunks;
        private IEnumerable<PersonStruct> _personStructs;
        [Params(10)] public int _count;

        [GlobalSetup]
        public void Setup()
        {
            _asList = new ArrayBuilderAsList<PersonStruct>();
            _useChunks = new ArrayBuilderWithChunksUsage<PersonStruct>();
            _personStructs = Enumerable.Range(1, _count).Select(x => new PersonStruct() {Age = 12, Name = "n"}).ToArray();
        }

        [Benchmark]
        public void AsList()
        {
            var r = _asList.GetArr(_personStructs);
        }
        
        
        [Benchmark]
        public void WithChunks()
        {
            var r = _useChunks.GetArr(_personStructs);
        }
    }

    public interface IArrayFromEnumerableBuilder<T>
    {
        T[] GetArr(IEnumerable<T> enumerable);
    }

    public class ArrayBuilderAsList<T> : IArrayFromEnumerableBuilder<T>
    {
        public T[] GetArr(IEnumerable<T> enumerable)
        {
            var length = 4;
            var arr = new T[4];
            var count = 0;
            using var enumerator = enumerable.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (count == length)
                {
                    var newArr = new T[length *= 2];
                    Array.Copy(arr, 0, newArr, 0, count);
                    arr = newArr;
                }

                arr[count++] = enumerator.Current;
            }

            return arr;
        }
    }

    public class ArrayBuilderWithChunksUsage<T> : IArrayFromEnumerableBuilder<T>
    {
        class Node
        {
            public T[] _value;
            public Node _next;
        }

        private Node _first;
        private Node _last;
        private T[] _current;
        private int _index;
        private int _count;

        public ArrayBuilderWithChunksUsage()
        {
            _current = new T[4];
        }

        public T[] GetArr(IEnumerable<T> enumerable)
        {
            using var enumerator = enumerable.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (_index == _current.Length)
                {
                    if (_first == null)
                    {
                        _first = new Node() {_value = _current};
                    }

                    else
                    {
                        if (_last == null)
                        {
                            _last = new Node() {_value = _current};
                            _first._next = _last;
                        }
                        else
                        {
                            _last._next = new Node {_value = _current};
                            _last = _last._next;
                        }
                    }

                    var newArr = new T[_index * 2];
                    Array.Copy(_current, 0, newArr, 0, _index);
                    _current = newArr;
                    _index = 0;
                }

                _current[_index++] = enumerator.Current;
                _count++;
            }

            var resultArr = new T[_count];
            var resultArrCount = 0;
            if (_first == null)
            {
                return _current;
            }

            for (var current = _first;
                current != null;
                current = current._next)
            {
                Array.Copy(
                    current._value,
                    0,
                    resultArr,
                    resultArrCount,
                    current._value.Length);

                resultArrCount += current._value.Length;
            }

            Array.Copy(
                _current,
                0,
                resultArr,
                resultArrCount,
                Math.Min(_current.Length, _count - resultArrCount));

            return resultArr;
        }
    }
}