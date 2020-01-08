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
      

        private IArrayFromEnumerableBuilder<PersonStruct> ArrayBuilderAsList;
        private IArrayFromEnumerableBuilder<PersonClass> ArrayBuilderWithChunksUsage;

        public ArrayBuildersTest()
        {
            ArrayBuilderAsList = new ArrayBuilderAsList<PersonStruct>();
            ArrayBuilderWithChunksUsage = new ArrayBuilderWithChunksUsage<PersonClass>();
        }

        [Theory]
        [ClassData(typeof(TestData))]
        public void SimpleTest(IArrayFromEnumerableBuilder<PersonStruct> builder,
            IEnumerable<PersonStruct> personStructs)
        {
            var actual = builder.GetArr(personStructs).Take(1000).ToList();
            var expectation = personStructs.ToList();
            actual
                .Should()
                .Equal(expectation);
        }
    }

    public class TestData : IEnumerable<object[]>
    {
        private PersonStruct[] structs;
        private PersonClass[] classes;

        public TestData()
        {
            var random = new Random();
            structs = Enumerable.Range(1, 1000).Select(x => new PersonStruct()
                    {Name = Guid.NewGuid().ToString(), Age = (byte) random.Next(0, 100)})
                .ToArray();


            classes = Enumerable.Range(1, 1000).Select(x => new PersonClass()
                    {Name = Guid.NewGuid().ToString(), Age = (byte) random.Next(0, 100)})
                .ToArray();
        }

        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] {new ArrayBuilderAsList<PersonStruct>(), structs};
            yield return new object[] {new ArrayBuilderWithChunksUsage<PersonStruct>(), structs};
        }

        private IEnumerable<object[]> ArrayBuilder()
        {
            yield return new object[] {new ArrayBuilderWithChunksUsage<PersonStruct>()};
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}