namespace contosoUniversity2020.Models
{
    public class CourseAssignment
    {
        public int InstructorID { get; set; } //Composite PK (with CourseID) , FK to Instructor Entity

        public int CourseID { get; set; } //Composite PK(with InstructorID) , FK to Course Entity

        /*
         * Note
         * The only way to identity a composite PK when using EF (Entity Framework) is by using the
         * Fluent API within the Database Context Class (which will be called SchoolContext in this case) 
         * It cannot be done using attributes
         * 
         */

        //Navigation Properties
        //Many-Many (this is the junction table between course and instructor)
        //Many Instructors teaching many courses
        //1 course to many Course Assignments
        //1 instructor to many course Assignments

        public virtual Instructor Instructor { get; set; }
        public virtual Course Course { get; set; }
    }
}