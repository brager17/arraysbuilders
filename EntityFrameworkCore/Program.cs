using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Moq;

namespace EntityFrameworkCore
{
    class Program
    {
        static void Main(string[] args)
        {


            var dbCtx = new PersonDbContext();
            var ps = dbCtx
                .Persons
                .Take(100);


            var array = ps.ToArray();
        }
    }

    public class PersonTester
    {
        public List<Person> Case1(IQueryable<Person> query)
        {
            var persons = query
                .ToList();

            foreach (var person in persons)
            {
                if (person.Age > 18)
                    person.IsAdult = true;
            }

            return persons;
        }


        public Person[] Case2(IQueryable<Person> query)
        {
            var persons = query
                .ToArray();

            foreach (var person in persons)
            {
                if (person.Age > 18)
                    person.IsAdult = true;
            }

            return persons;
        }
    }

    public class Person
    {
        public int Id { get; set; }
        public int Age { get; set; }
        public bool IsAdult { get; set; }
        public string Name { get; set; }

        public Person()
        {
        }
    }

    public class PersonDbContext : DbContext
    {
        public PersonDbContext() : base(new DbContextOptionsBuilder<PersonDbContext>().UseSqlServer(C).Options)
        {
        }

        public DbSet<Person> Persons { get; set; }
        public const string C = "Server=.;Database=test;User Id=sa;Password=reallyStrongPwd123;";
    }
}