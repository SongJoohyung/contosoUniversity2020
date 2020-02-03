using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace contosoUniversity2020.Models
{
    public class Course
    {
        //remove the default Identity Property (autonumber feature)
        //choices are - Computed, Identity, or None
        //Computed: Database generates a value when a row is inserted or updated
        //Identity: Database generated a value when row is inserted
        //None: Database does not generate a valued
        
        // We want the use to enter the course id manually
        [DatabaseGenerated(DatabaseGeneratedOption.None)]

        [Display(Name = "Course Number")]
        [Required]
        public int CourseID { get; set; }//PK
        
        [Required]
        [StringLength(50,MinimumLength= 3)]
        public string Title { get; set; }
        
        [Range(3,6)]
        public int Credits { get; set; }
        
        [Display(Name = "Department")]
        public int DepartmentID { get; set; }//FK to Department entity

        //Read only property: return the course id and title
        public string CourseIdTitle
        {
            get
            {
                return CourseID + ": " + Title;
                //1021: Intro to C# programming
            }
        }

        public virtual ICollection<Enrollment> Enrollments { get; set; } //1 course with many enrollments
    
        public virtual Department Department { get; set; }//A course can only beling to at most one department
    }
}
