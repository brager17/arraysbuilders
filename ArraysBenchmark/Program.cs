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
            var c = new ClassBenchmarkWithStaticPerson();
            c.ToArrayClass();
            // BenchmarkRunner.Run<CopyArrayStructureVsClasses>();
            // BenchmarkRunner.Run<CompareArrayBuilderAndToList>();
            // BenchmarkRunner.Run<ToArrayBenchmark>();
            // Console.WriteLine(Marshal.SizeOf<PersonClass>());
            // BenchmarkRunner.Run<ToListToArrayQueryableBenchmark>();
            // BenchmarkRunner.Run<IntBenchmarks>();
            // BenchmarkRunner.Run<StringBenchmark>();
            // var q = BenchmarkRunner.Run<ClassBenchmarkWithStaticPerson>();
            // var totalOperations = q.Reports.Select(x => x.GcStats.TotalOperations);
            // foreach (var uten in totalOperations)
            // {
                // Console.WriteLine(uten);
            // }

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
}

