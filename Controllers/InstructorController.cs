using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using contosoUniversity2020.Data;
using contosoUniversity2020.Models;
using contosoUniversity2020.Models.SchoolViewModels;
using Microsoft.AspNetCore.Authorization;

namespace contosoUniversity2020.Controllers
{
    //mwilliams: Part 12: Authorization (securing admin controllers)
    [Authorize(Roles = "Admin")]

    //mwilliams: Part 4: Scaffold Models and Customize Views

    public class InstructorController : Controller
    {
        private readonly SchoolContext _context;

        public InstructorController(SchoolContext context)
        {
            _context = context;
        }

        // GET: Instructors

        //mwilliams: Part 8: Creating View models
        //1. Instructor related records (instructors, courses, enrollments)
        //    Which is in the ViewModel (InstructorIndexData)
        //    Replace the old Index method with this one
        public async Task<IActionResult> Index(int? id, int? courseID) //Part 4: Which instructor was selected (added the id param)

        {
            var viewModel = new InstructorDateGroup();
            viewModel.Instructors = await _context.Instructor
                .Include(i => i.OfficeAssignment) //1. Part1: Get Instructors including office assignment   
                .Include(i => i.Courses)          //2. Part2. Get the Courses
                .ThenInclude(i => i.Course)         //   Have to get the Course Entity out of the Courses JOIN Entity
                .ThenInclude(i => i.Department)     //3. Part3. Get the Department (to show the Department Name) 
                .ToListAsync();
            //============================= INSTRUCTOR SELECTED========================
            if (id != null)
            { 
                    Instructor instructor = viewModel.Instructors.Where(
                        i => i.ID == id.Value).SingleOrDefault(); //Get a single instructor that matches id param
            if (instructor == null)
            {
                return NotFound();
            }
            viewModel.Courses = instructor.Courses.Select(s => s.Course);
            //Get the instructor name for display within view (using ViewData)
            ViewData["InstructorName"] = instructor.FullName;

                //Return the instructor id (id) back to the view for highlighting the selected row
                ViewData["InstructorID"] = id.Value;

            }
        //============================= END INSTRUCTOR SELECTED====================

        //=================================COURSE SELECTED=========================
        if (courseID != null)
        {//if the courseID course param is passed in
         //Get the enrollment data
                _context.Enrollments.Include(i => i.Student)
                        .Where(c => c.CourseID == courseID.Value).Load();//Explicit Loading
                //Only enrollments for a single selected course (courseID = 1050)
                //We do not want all enrollment in this case, for example:
                //viewModel.Enrollments = _context.Enrollments

                var enrollments = viewModel.Courses
                    .Where(x => x.CourseID == courseID).SingleOrDefault();

                if (enrollments == null)
                {
                    return NotFound();
                }
                viewModel.Enrollments = enrollments.Enrollments;

                //Pass back the course id to the view using ViewData for selected row CSS
                ViewData["CourseID"] = courseID.Value; //this is the URL parameter called courseID

        }

        //=============================END COURSE SELECTED========================
         
            //return with view 
            return View(viewModel);
        }
        //public async Task<IActionResult> Index()
        //{
        //  return View(await _context.Instructor.ToListAsync());
        //}

         
        // GET: Instructors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructor = await _context.Instructor
                .FirstOrDefaultAsync(m => m.ID == id);
            if (instructor == null)
            {
                return NotFound();
            }

            return View(instructor);
        }

        // GET: Instructors/Create
        //mwilliams Part 9: Update related records(instructor)
        public IActionResult Create()
        {
            //new code
            //Get course assignments (for this instructor - the one we are currently editing)
            //Create the instructor object
            var instructor = new Instructor();
            //create list of courses 
            instructor.Courses = new List<CourseAssignment>();
            //part2: Get course assignments (for this instructor - the one we are currently editing)
            PopulateAssignmentCourseData(instructor);

            //end part2
            return View();
        }

        // POST: Instructors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("HireDate,OfficeAssignment,FirstName,LastName,Email,Address,City,Province,PostalCode")] Instructor instructor,
                                            string[]selectedCourse /*mwilliams: added course assignment*/ )
        {
            //mwilliams Part 9: Update related records (course Assignment)
            //check for selectedCourse
            if(selectedCourse!= null)
            {
                //some checkboxes have been checked - create a list of course assignment
                instructor.Courses = new List<CourseAssignment>();
                foreach(var course in selectedCourse)
                {
                    //Populate the CourseAssignment object
                    var courseToAdd = new CourseAssignment
                    {
                        InstructorID = instructor.ID,
                        CourseID = int.Parse(course)
                    };
                    instructor.Courses.Add(courseToAdd); 
                }
            }

            if (ModelState.IsValid)
            {
                _context.Add(instructor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(instructor);
        }

        // GET: Instructors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //mwilliams Part 9: Update related records (instructor)
            //var instructor = await _context.Instructor.FindAsync(id);
            var instructor = await _context.Instructor
                             .Include(i => i.OfficeAssignment)//include office assignment (part 1)
                             .Include(i => i.Courses)//include course assignment (part 2)
                             .SingleOrDefaultAsync(i => i.ID == id.Value);//get a single insturctor
            if (instructor == null)
            {
                return NotFound();
            }

            //part2: Get course assignments (for this instructor - the one we are currently editing)
            PopulateAssignmentCourseData(instructor);

            //end part2
            return View(instructor);
        }

        private void PopulateAssignmentCourseData(Instructor instructor)
        {
            //1. Get all Courses
            var allCourses = _context.Courses;

            //2. Create a hash set of instructor courses (Hashset of integers with course id)
            var instructorCourses = new HashSet<int>(instructor.Courses.Select(c => c.CourseID));

            var viewModel = new List<AssignedCourseData>();

            foreach (var course in allCourses)
            {
                viewModel.Add(new AssignedCourseData
                {
                    CourseID = course.CourseID,
                    Title = course.Title,
                    Assigned = instructorCourses.Contains(course.CourseID)

                });
            }
            ViewData["Courses"] = viewModel;
        }
        // POST: Instructors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("HireDate,ID,FirstName,LastName,Email,Address,City,Province,PostalCode")] Instructor instructor)
        public async Task<IActionResult> Edit(int? id,string[] selectedCourse)
        {
            if (id==null)
            {
                return NotFound();
            }

            var instructorToUpdate = await _context.Instructor
                .Include(i => i.OfficeAssignment)
                .Include(i => i.Courses) //include courses (for course assignment) - part2
                .ThenInclude(i => i.Course) //include course for update - part2
                .SingleOrDefaultAsync(i => i.ID == id.Value); //for only one instructor

            //try to update the instructor
            if(await TryUpdateModelAsync<Instructor>(
                instructorToUpdate,"",
                i=>i.FirstName,i=>i.LastName,i=>i.Address,
                i => i.City, i => i.Province, i => i.PostalCode,
                i => i.Email, i => i.HireDate, 
                i => i.OfficeAssignment)
                )
            {
                // Check for empty office location
                if (string.IsNullOrWhiteSpace(instructorToUpdate.OfficeAssignment.Location))
                {
                    instructorToUpdate.OfficeAssignment = null;    
                }

                //part 2: Course Assignment
                UpdateInstructorCourses(selectedCourse, instructorToUpdate);
                //Part 2:  Update Related Data (Instructor Assigned Courses)
                
               
            }
            //save changes back to database
            if (ModelState.IsValid)
            {
                try
                {
                    
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Unable to save changes!");
                }
                return RedirectToAction(nameof(Index));
            }
            return View(instructorToUpdate);
        }

        //Part 2:  Update Related Data (Instructor Assigned Courses)
        private void UpdateInstructorCourses(string[] selectedCourse, Instructor instructorToUpdate)
        {
            if (selectedCourse == null)
            {
                //If no checkboxes were selected, initialize the Courses navigation property
                //with an empty collection and return
                instructorToUpdate.Courses = new List<CourseAssignment>();
                return;
            }

            //To facilitate efficient lookups, 2 collections will be stored in HashSet objects
            //: selectedCourseHS ->  selected course (hashset of checkboxe selections)
            //: instructorCourses -> instructor courses (hashset of courses assigned to instructor)
            var selectedCourseHS = new HashSet<string>(selectedCourse);
            var instructorCourses = new HashSet<int>
                (instructorToUpdate.Courses.Select(c => c.Course.CourseID));

            //Loop through all courses in the database and check each course against the ones
            //currently assigned to the instructor versus the ones that were selected in the
            //view
            foreach (var course in _context.Courses)//Loop all courses
            {
                //CONDITION 1:
                //If the checkbox for a course was selected but the course isn't in the 
                //Instructor.Courses navigation property, the course is added to the collection
                //in the navigation property
                if (selectedCourseHS.Contains(course.CourseID.ToString()))
                {
                    if (!instructorCourses.Contains(course.CourseID))
                    {
                        instructorToUpdate.Courses.Add(new CourseAssignment
                        {
                            InstructorID = instructorToUpdate.ID,
                            CourseID = course.CourseID
                        });
                    }
                }
                //CONDITION 2:
                //If the check box for a course wasn't selected, but the course is in the 
                //Instructor.Courses navigation property, the course is removed 
                //from the navigation property.
                else
                {
                    if (instructorCourses.Contains(course.CourseID))
                    {
                        CourseAssignment courseToRemove =
                            instructorToUpdate.Courses
                            .SingleOrDefault(i => i.CourseID == course.CourseID);
                        _context.Remove(courseToRemove);
                    }
                }

            }//end foreach
        }

        // GET: Instructors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructor = await _context.Instructor
                .FirstOrDefaultAsync(m => m.ID == id);
            if (instructor == null)
            {
                return NotFound();
            }

            return View(instructor);
        }

        // POST: Instructors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var instructor = await _context.Instructor.FindAsync(id);
            try
            {
                _context.Instructor.Remove(instructor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index)); 
            }
            //catch (DbUpdateException ex)
            catch (Microsoft.Data.SqlClient.SqlException ex)
            {
                
                if( ((Microsoft.Data.SqlClient.SqlException)ex.InnerException).Number==547)
                {
                    //FK constraint error (cascade restrict)
                    ModelState.AddModelError("", "Unable to delete instructor due to related records!");
                }
                else
                {
                    //Some other error
                    ModelState.AddModelError("", "Unable to delete instructor due to a system error!");
                }
                
            }
            //failed to update return back to view attacking the instrcutor object
            return View(instructor);
        }

        private bool InstructorExists(int id)
        {
            return _context.Instructor.Any(e => e.ID == id);
        }
    }
}
