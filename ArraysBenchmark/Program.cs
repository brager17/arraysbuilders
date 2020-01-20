using System;
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
            // BenchmarkRunner.Run<CopyArrayStructureVsClasses>();
            // BenchmarkRunner.Run<CompareArrayBuilderAndToList>();
            // BenchmarkRunner.Run<ToArrayBenchmark>();
            // Console.WriteLine(Marshal.SizeOf<PersonClass>());
            // BenchmarkRunner.Run<ToListToArrayQueryableBenchmark>();
            // BenchmarkRunner.Run<IntBenchmarks>();
            // BenchmarkRunner.Run<StringBenchmark>();
            var q = BenchmarkRunner.Run<ClassBenchmarkWithStaticPerson>();
            var totalOperations = q.Reports.Select(x => x.GcStats.TotalOperations);
            foreach (var uten in totalOperations)
            {
                Console.WriteLine(uten);
            }
            // BenchmarkRunner.Run<StructBenchmark>();
            // var b = new CompareToArrayVsToList();
            // Array.Copy(new int[0], new int[0], 1);
            // b._count = 10;
            // b.ToListClass();
        }
    }

    [MemoryDiagnoser] public class MemoryTest
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
            Marshal.SizeOf(typeof(IntPtr));
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
            using (var enumerator = enumerable.GetEnumerator())
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


    [MemoryDiagnoser] public class CopyArrayStructureVsClasses
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


    [MarkdownExporter, AsciiDocExporter, HtmlExporter, CsvExporter, RPlotExporter, PlainExporter] [MemoryDiagnoser]
    public class StructBenchmark
    {
        private IEnumerable<PersonStruct> EnumerableStructs;


        #region params

        [Params(
            10000,
            20000,
            30000,
            40000,
            50000,
            60000,
            70000,
            80000,
            90000,
            100000,
            110000,
            120000,
            130000,
            140000,
            150000,
            160000,
            170000,
            180000,
            190000,
            200000,
            21000,
            220000,
            230000,
            240000,
            250000,
            260000,
            270000,
            280000,
            290000,
            300000,
            31000,
            320000,
            330000,
            340000,
            350000,
            360000,
            370000,
            380000,
            390000,
            400000,
            41000,
            420000,
            430000,
            440000,
            450000,
            460000,
            470000,
            480000,
            490000,
            500000,
            51000,
            520000,
            530000,
            540000,
            550000,
            560000,
            570000,
            580000,
            590000,
            600000,
            610000,
            620000,
            630000,
            640000,
            650000,
            660000,
            670000,
            680000,
            690000,
            700000,
            710000,
            720000,
            730000,
            740000,
            750000,
            760000,
            770000,
            780000,
            790000,
            800000,
            810000,
            820000,
            830000,
            840000,
            850000,
            860000,
            870000,
            880000,
            890000,
            900000,
            910000,
            920000,
            930000,
            940000,
            950000,
            960000,
            970000,
            980000,
            990000,
            1000000,
            1000010,
            1000020,
            1000030,
            1000040,
            1000050,
            1000060,
            1000070,
            1000080,
            1000090,
            1000100,
            1000110
        )]

        #endregion

        public int _count;

        public StructBenchmark()
        {
            EnumerableStructs = GetEnumerable();
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

        [Benchmark]
        public void ToArrayStruct()
        {
            var r = EnumerableStructs.ToArray();
        }


        [Benchmark]
        public void ToListStruct()
        {
            var r = EnumerableStructs.ToList();
        }
    }

    [MemoryDiagnoser] public class ToListToArrayQueryableBenchmark
    {
        private PersonTester _personTester;
        private IQueryable<Person> _queryable;

        [Params(10_000)]
        public int Count;

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