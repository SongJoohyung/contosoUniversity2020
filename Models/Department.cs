using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace contosoUniversity2020.Models
{
    public class Department
    {
        public int DepartmentID { get; set; }

        [Required]
        [StringLength(60, MinimumLength =3)]
        public string Name { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName= "money")]
        public decimal Budget { get; set; }

    
        [Display(Name = "Date Created")]
        [DisplayFormat(DataFormatString = "(0:yyyy-MM-dd)", ApplyFormatInEditMode = true)]
        public DateTime CreatedDate { get; set; }
        [Display(Name = "Administrator")]

        public int InstructorID { get; set; } //A Department MAY have an Administrator (instructor)
                                              // and an Administrator is always an instructor

        public virtual Instructor Administrator { get; set; }

        public virtual ICollection<Course> Courses { get; set; }
    }
}