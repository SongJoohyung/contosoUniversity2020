using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace contosoUniversity2020.Models.SchoolViewModels
{
    public class InstructorDateGroup
    {
        //mwilliams: Part 8: Creating View models
        //1. Instructor related records
        //   - Instructor, Course, Enrollment

/* The Instructor Index View will show data from three diffrent tables
 * (model|entities), so for this reason we are creating this ViewModel.
 * It will include the following:
 *   - Instructor 
 *   - Course 
 *   - Enrollment
 
      */

public IEnumerable<Instructor> Instructors { get; set; }

public IEnumerable<Course> Courses { get; set; }

public IEnumerable<Enrollment> Enrollments { get; set; }
}
}
