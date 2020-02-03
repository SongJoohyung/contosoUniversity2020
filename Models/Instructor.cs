using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace contosoUniversity2020.Models
{
    public class Instructor:Person
    {
           
        [DataType(DataType.Date)]//This will create a data picker in HTML
        [Display(Name = "Hire Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime HireDate { get; set; }

        public virtual ICollection<CourseAssignment> Courses { get; set; }

        public virtual OfficeAssignment OfficeAssignment { get; set; }
    }
}
    