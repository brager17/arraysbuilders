﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Principal;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using EntityFrameworkCore;

namespace ConsoleApp1
{
    public struct PersonStruct
    {
        public string Name;
        public string Name1;
        public string Name2;
        public byte Age;
        public byte Age1;
        public byte Age2;
    }

    public class PersonClass
    {
        public string Name;
        public string Name1;
        public string Name2;
        public byte Age;
        public byte Age1;
        public byte Age2;
    }

    public struct Struct
    {
        public int Test;
        public int Test1;
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(sizeof(decimal));
            // BenchmarkRunner.Run<CopyArrayStructureVsClasses>();
            // BenchmarkRunner.Run<CompareArrayBuilderAndToList>();
            // BenchmarkRunner.Run<ToArrayBenchmark>();
            // Console.WriteLine(Marshal.SizeOf<PersonClass>());
            // BenchmarkRunner.Run<ToListToArrayQueryableBenchmark>();
            BenchmarkRunner.Run<CompareToArrayVsToList>();
        }
    }

    [MemoryDiagnoser]
    public class MemoryTest
    {
        [Benchmark]
        public PersonClass ClassSize()
        {
            return new PersonClass();
        }

        private void NoAllocatedMethod(PersonClass personClass)
        {
            var q = personClass.Age;
        }

        //
        // [Benchmark]
        // public void StructSize()
        // {
        //     var arr = new PersonClass[10];
        //     arr[0] = new PersonClass();
        //     arr[1] = new PersonClass();
        //     arr[2] = new PersonClass();
        //     arr[3] = new PersonClass();
        //     arr[4] = new PersonClass();
        //     arr[5] = new PersonClass();
        //     arr[6] = new PersonClass();
        //     arr[7] = new PersonClass();
        //     arr[8] = new PersonClass();
        //     arr[9] = new PersonClass();
        // }

        public void IntPtrTest()
        {
            unsafe
            {
                System.Runtime.InteropServices.Marshal.SizeOf(typeof(IntPtr));
            }
        }
    }

    [MemoryDiagnoser]
    public class CompareArrayBuilderAndToList
    {
        private IEnumerable<PersonStruct> Enumerable;
        private IEnumerable<PersonClass> EnumerableClasses;

        [Params(10, 100, 1000, 10_000, 100_000)]
        public int _count;

        private IEnumerable<PersonStruct> GetEnumerable()
        {
            for (var i = 0; i < _count; i++)
            {
                yield return new PersonStruct
                {
                    Name = "name",
                    Age = 12
                };
            }
        }

        private IEnumerable<PersonClass> GetEnumerableClasses()
        {
            for (var i = 0; i < _count; i++)
            {
                yield return new PersonClass()
                {
                    Name = "name",
                    Age = 12
                };
            }
        }

        [GlobalSetup]
        public void Setup()
        {
            Enumerable = GetEnumerable();
            EnumerableClasses = GetEnumerableClasses();
        }

        [Benchmark]
        public void ToListClasses()
        {
            var r = EnumerableClasses.ToList();
        }

        [Benchmark]
        public void ToListCopyClasses()
        {
            var r = EnumerableClasses.ToListCopy();
        }

        [Benchmark]
        public void ToListStructures()
        {
            var r = Enumerable.ToList();
        }

        [Benchmark]
        public void ToListCopyStructures()
        {
            var a = 0X7FEFFFFF;
            var r = Enumerable.ToListCopy();
        }
    }

    [MemoryDiagnoser]
    public class ToArrayBenchmark
    {
        private IEnumerable<PersonStruct> Enumerable;
        private IEnumerable<PersonClass> EnumerableClasses;

        [Params(10, 100, 1000, 10_000, 100_000)]
        public int _count;

        private IEnumerable<PersonStruct> GetEnumerable()
        {
            for (var i = 0; i < _count; i++)
            {
                yield return new PersonStruct
                {
                    Name = "name",
                    Age = 12
                };
            }
        }

        private IEnumerable<PersonClass> GetEnumerableClasses()
        {
            for (var i = 0; i < _count; i++)
            {
                yield return new PersonClass()
                {
                    Name = "name",
                    Age = 12
                };
            }
        }

        [GlobalSetup]
        public void Setup()
        {
            Enumerable = GetEnumerable();
            EnumerableClasses = GetEnumerableClasses();
        }

        [Benchmark]
        public void OriginalToArrayClasses()
        {
            var r = EnumerableClasses.ToArray();
        }


        [Benchmark]
        public void CopyToArrayClasses()
        {
            var r = EnumerableClasses.ToArrayCopy();
        }


        [Benchmark]
        public void OriginalToArrayStructure()
        {
            var r = Enumerable.ToArray();
        }


        [Benchmark]
        public void CopyToArrayStructure()
        {
            var r = Enumerable.ToArrayCopy();
        }
    }

    public interface IArrayFromEnumerableBuilder<T>
    {
        T[] GetArr(IEnumerable<T> enumerable);
    }

    public class ArrayBuilderAsList<T> : IArrayFromEnumerableBuilder<T>
    {
        public T[] _arr;
        private int _count;

        public ArrayBuilderAsList(IEnumerable<T> enumerable)
        {
            _arr = new T[4];
            GetArr(enumerable);
        }

        public T[] GetArr(IEnumerable<T> enumerable)
        {
            var arr = _arr;

            using (var enumerator = enumerable.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    Add(enumerator.Current);
                }
            }

            return arr;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Add(T value)
        {
            var arr = _arr;
            var count = _count;
            if ((uint) count < (uint) arr.Length)
            {
                _count = count + 1;
                arr[count] = value;
            }
            else
            {
                Ensure(value);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void Ensure(T value)
        {
            var count = _count;
            EnsureCapacity(count);
            _count = count + 1;
            _arr[count] = value;
        }

        private void EnsureCapacity(int count)
        {
            var newArr = new T[count * 2];
            Array.Copy(_arr, 0, newArr, 0, count);
            _arr = newArr;
        }
    }

    public class PersonClassArrayBuilder
    {
        private PersonClass[] _arr;
        private int _count;

        public PersonClass[] GetArr(IEnumerable<PersonClass> enumerable)
        {
            _arr = new PersonClass[4];
            var arr = _arr;


            using (var enumerator = enumerable.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    // var count = _count;
                    // if ((uint) count < (uint) arr.Length)
                    // {
                    //     // arr[count] = enumerator.Current;
                    //     // _count = count + 1;
                    // }
                    // else
                    // {
                    //     Ensure( enumerator.Current);
                    //     arr = _arr;
                    // }
                }
            }

            return arr;
        }

        private void Ensure(PersonClass value)
        {
            // var arr = _arr;
            // var count = _count;
            var newArr = new PersonClass[_arr.Length * 2];
            Array.Copy(_arr, 0, newArr, 0, _count);
            // newArr[count] = value;
            // _count = count + 1;
            _arr = newArr;
        }
    }

    public class ArraysStructuresVSArrayClasses
    {
        [Benchmark]
        public void ArrayStructures()
        {
            PersonStruct[] a = new PersonStruct[1000000000];
        }

        [Benchmark]
        public void ArrayClasses()
        {
            PersonClass[] a = new PersonClass[1000000000];
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

    public static class ArrayBuilderExtensions
    {
        public static T[] ToListCopy<T>(this IEnumerable<T> enumerable)
        {
            return new MyArrayBuilderFromOneMethod().GetArray(enumerable);
        }

        public static T[] ToArrayCopy<T>(this IEnumerable<T> enumerable)
        {
            return new MyArrayBuilderWithChunks().GetArray(enumerable);
        }
    }

    [MemoryDiagnoser]
    public class CopyArrayStructureVsClasses
    {
        private PersonClass[] _classes;
        private PersonStruct[] _structs;

        [Params(10, 100, 1000, 10_000, 100_000)]
        public int count;

        [GlobalSetup]
        public void Setup()
        {
            _classes = Enumerable.Range(1, count).Select(x => new PersonClass()).ToArray();
            _structs = Enumerable.Range(1, count).Select(x => new PersonStruct()).ToArray();
        }

        [Benchmark]
        public void ClassCopy()
        {
            var classes = new PersonClass[count];
            Array.Copy(_classes, 0, classes, 0, count);
        }


        [Benchmark]
        public void StructCopy()
        {
            var structs = new PersonStruct[count];
            Array.Copy(_structs, 0, structs, 0, count);
        }
    }

    public class CompareToArrayVsToList
    {
        private IEnumerable<PersonStruct> EnumerableStructs;
        private IEnumerable<PersonClass> EnumerableClasses;
        private IEnumerable<int> EnumerableInts;
        private IEnumerable<string> EnumerableStrings;

        [Params(
            10,
            16, 17,
            24,
            32, 33,
            48,
            64, 65,
            96,
            128, 129,
            192,
            256, 257,
            384,
            512, 513,
            768,
            1024, 1025,
            1536,
            2048, 2049,
            3072,
            4096, 4097,
            6144,
            8182, 8183,
            12288,
            16384, 16385,
            24576,
            262144, 262145,
            393216,
            524288, 524289,
            786432,
            1048576, 1048567,
            1572864,
            2097152, 2097153,
            3145728,
            4194304, 4194305,
            6291456,
            8388608, 8388609,
            12582912,
            16777216, 16777217
        )]
        public int _count;

        public CompareToArrayVsToList()
        {
            EnumerableStructs = GetEnumerable();
            EnumerableClasses = GetEnumerableClasses();
            EnumerableInts = GetEnumerableInts();
            EnumerableStrings = GetEnumerableStrings();
        }

        private IEnumerable<PersonStruct> GetEnumerable()
        {
            for (var i = 0; i < _count; i++)
            {
                yield return new PersonStruct
                {
                    Name = "name",
                    Age = 12
                };
            }
        }

        private IEnumerable<PersonClass> GetEnumerableClasses()
        {
            for (var i = 0; i < _count; i++)
            {
                yield return new PersonClass()
                {
                    Name = "name",
                    Age = 12
                };
            }
        }

        private IEnumerable<int> GetEnumerableInts()
        {
            for (var i = 0; i < _count; i++)
            {
                yield return 1;
            }
        }

        private IEnumerable<string> GetEnumerableStrings()
        {
            for (var i = 0; i < _count; i++)
            {
                yield return "string";
            }
        }

        [Benchmark]
        public void ToArrayInt()
        {
            var r = EnumerableInts.ToArray();
        }

        [Benchmark]
        public void ToArrayString()
        {
            var r = EnumerableStrings.ToArray();
        }


        [Benchmark]
        public void ToArrayClass()
        {
            var r = EnumerableClasses.ToArray();
        }


        [Benchmark]
        public void ToArrayStruct()
        {
            var r = EnumerableStructs.ToArray();
        }

        [Benchmark]
        public void ToListInt()
        {
            var r = EnumerableInts.ToList();
        }

        [Benchmark]
        public void ToListString()
        {
            var r = EnumerableStrings.ToList();
        }


        [Benchmark]
        public void ToListClass()
        {
            var r = EnumerableClasses.ToList();
        }


        [Benchmark]
        public void ToListStruct()
        {
            var r = EnumerableStructs.ToList();
        }
    }

    [MemoryDiagnoser]
    public class ToListToArrayQueryableBenchmark
    {
        private PersonTester _personTester;
        private IQueryable<Person> _queryable;
        [Params(10_000)] public int Count;

        [GlobalSetup]
        public void Setup()
        {
            _personTester = new PersonTester();
            _queryable = GetPerson().AsQueryable();
        }

        private IEnumerable<Person> GetPerson()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return new Person();
            }
        }

        [Benchmark]
        public void ToList()
        {
            var r = _personTester.Case1(_queryable);
        }

        [Benchmark]
        public void ToArray()
        {
            var r = _personTester.Case2(_queryable);
        }
    }
}