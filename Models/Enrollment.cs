using contosoUniversity2020.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace contosoUniversity2020.Models
{
    public class Enrollment
    {
        public int EnrollmentID { get; set; }
        [Display(Name = "Course")]
        public int CourseID { get; set; } //FK to Course Identity
        [Display(Name = "Student")]
        public int StudentID { get; set; } // FK to student(ID) Entity

        [DisplayFormat(NullDisplayText = "No Grade Yet")] //Display "No Grade Yet" when Grade is Null
        public Grade? Grade { get; set; }//? means nullable, we don't strart with a grade upon a registration

        public virtual Student Student { get; set; }

        public virtual Course Course { get; set; }
    }

public enum Grade
{
    A, B, C, D, F   
}
}
