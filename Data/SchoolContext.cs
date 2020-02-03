using contosoUniversity2020.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace contosoUniversity2020.Data
{
    //mwilliams: Part 3: Create the database context
    public class SchoolContext:DbContext
    { 
        public SchoolContext(DbContextOptions<SchoolContext>options):base(options)
        {
            
        }

        //Specify Entiset Sets - corresponding to the database tables that will
        //be created upon database migration. Each single entity corresponds
        //to a row in a table
        public DbSet<Person> People { get; set; }

        public DbSet<Student> Students { get; set; }

        public DbSet<Instructor> Instructor { get; set; }

        public DbSet<Enrollment> Enrollments { get; set; }

        public DbSet<Course> Courses { get; set; }

        public DbSet<CourseAssignment> CourseAssignments { get; set; }

        public DbSet<Department> Departments { get; set; }

        public DbSet<OfficeAssignment> OfficeAssignments { get; set; }

        /*
         * Plural table names are the default:
         * When the database is created (from migration process). EF creates
         * tables that have the same as the DbSet property names.
         * 
         * -- Just a developer debate (plural vs singular table names)
         * We can leave it as plural or overriding the behavior using
         * the Fluent API 
         */

        //Use the Fluent API to make table names singular 
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>().ToTable("Person");
            modelBuilder.Entity<Enrollment>().ToTable("Enrollment");
            modelBuilder.Entity<Course>().ToTable("Course");
            modelBuilder.Entity<CourseAssignment>().ToTable("CourseAssignment");
            modelBuilder.Entity<Department>().ToTable("Department");
            modelBuilder.Entity<OfficeAssignment>().ToTable("OfficeAssignment");

            modelBuilder.Entity<CourseAssignment>()
                .HasKey(c => new { c.CourseID, c.InstructorID });
        }
    }
}
