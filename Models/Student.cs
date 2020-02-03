using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace contosoUniversity2020.Models
{
    //mwilliams: Part 2: Create the data models
    //2. Create the student class (inheriting from Person)
    public class Student:Person
    {   
        [DataType(DataType.Date)]//This will create a data picker in HTML
        [Display(Name = "Enrollment Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode=true)]
        public DateTime EnrollmentDate { get; set; }

        public virtual ICollection<Enrollment> Enrollments { get; set; }
    }
}
