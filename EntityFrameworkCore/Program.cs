using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace EntityFrameworkCore
{
    class Program
    {
        static void Main(string[] args)
        {
            var c = new PersonDbContext();
            c.Persons.Add(new Person() {Name = "name"});
            c.SaveChanges();


            var ps = c.Persons
                .Where(x => x.Name != "namename")
                .Take(100);


            var w = ps.ToList();
            
        }
    }

    public class Person
    {
        public int Id { get; set; }
        public int Age { get; set; }
        public string Name { get; set; }
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