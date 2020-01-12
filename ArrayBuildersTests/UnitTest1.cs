using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ConsoleApp1;
using FluentAssertions;
using Xunit;

namespace ArrayBuildersTests
{
    public class ArrayBuildersTest
    {
      


        public ArrayBuildersTest()
        {
        }

        [Theory]
        [ClassData(typeof(TestData))]
        public void SimpleTest(dynamic builder,
            IEnumerable<PersonStruct> personStructs)
        {
            var actual = (builder.GetArray(personStructs) as IEnumerable<PersonStruct>).Take(10000).ToList();
            var expectation = personStructs.ToList();
            actual
                .Should()
                .Equal(expectation);
        }

        public void CastArray()
        {
            var a = new int[] {1, 2, 3};
            var collection = a as ICollection<int>;
        }
        
       
    }

    public class TestData : IEnumerable<object[]>
    {
        private PersonStruct[] structs;
        private PersonClass[] classes;

        public TestData()
        {
            var random = new Random();
            structs = Enumerable.Range(1, 10000).Select(x => new PersonStruct()
                    {Name = Guid.NewGuid().ToString(), Age = (byte) random.Next(0, 100)})
                .ToArray();


            classes = Enumerable.Range(1, 10000).Select(x => new PersonClass()
                    {Name = Guid.NewGuid().ToString(), Age = (byte) random.Next(0, 100)})
                .ToArray();
        }

        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] {new MyArrayBuilderWithChunks(), structs};
            yield return new object[] {new MyArrayBuilderFromOneMethod(), structs};
        }

        private IEnumerable<object[]> ArrayBuilder()
        {
            yield return new object[] {new ArrayBuilderWithChunksUsage<PersonStruct>()};
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public List<T> ToList<T>(ICollection<T> collection)
        {
            // внутри делает то же самое, что и ToArray
            var list = new List<T>(collection);
            return list;
        }
        
        public T[] ToArray<T>(ICollection<T> collection)
        {
            var arr = new T[collection.Count];
            collection.CopyTo(arr,0);
            IQueryable<int> queryable;
            new int[] {1}.AsQueryable().ToList();
            return arr;
            
            
        }
    }
}