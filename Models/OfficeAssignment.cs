using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace contosoUniversity2020.Models

{
    public class OfficeAssignment
    {
        [Key]
        public int InstructorID { get; set; }//PK

        [Display(Name = "Office Location")]
        [StringLength(60)]

        public string Location { get; set; }

        public virtual Instructor Instructor { get; set; }// 1:1 (One Instructor assigned to one office)
    }
}