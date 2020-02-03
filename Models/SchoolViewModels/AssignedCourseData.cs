using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace contosoUniversity2020.Models.SchoolViewModels
{
    public class AssignedCourseData
    {
        /* This ViewModel Class will be used to add course
         * assignments to the Instructor (Edi|Create) pages.
         * It will provide a list of course checkboxes with
         * CourseID and CourseTitle as well as an indicator
         * that the instructor is assigned or not assigned to a
         * particular course
         * 
         * The view will generate a checkboxes for each course
         *  <input type="checkbox" name="selectedCourses
         *  id="1200" value="1200" checked>
         *  Calculus
         *  So we will need to create 3 properties
         *  -one for the course title
         *  -a second for the course id
         *  -a third for checked|not checked flag
         */
         public int CourseID { get; set; }

         public string Title { get; set; }
        
         public bool Assigned { get; set; } 
    }
}
