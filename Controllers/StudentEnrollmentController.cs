using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using contosoUniversity2020.Data;
using contosoUniversity2020.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace contosoUniversity2020.Controllers
{
    [Authorize(Roles="Student")]
    public class StudentEnrollmentController : Controller
    {
        private readonly SchoolContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public StudentEnrollmentController(SchoolContext context,
                                            UserManager<IdentityUser> userManager
                                            )
        {
            _context = context;
            _userManager = userManager;
        }

        private Task<IdentityUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }

        // GET: StudentEnrollment
        public async Task<IActionResult> Index()
        {
            //5. mwilliams: retrieve currently logged in user
            var user = await GetCurrentUserAsync();

            if(user==null)
            {
                return NotFound(); //note: could return some king of error view here
            }

            //6. mwilliams: locate the logged in user (student) within Student Entity
            var student = await _context.Students
                .Include(s => s.Enrollments)//need enrollments for this student
                .ThenInclude(s => s.Course)//need course info too
                .AsNoTracking() //remove caching of database object context
                .SingleOrDefaultAsync(s => s.Email == user.Email);//only for the logged in student

            //7. mwilliams: Courses Enrolled (courses that current student is enrolled in)
            var studentEnrollments = _context.Enrollments
                .Include(c => c.Course) //need course in
                .Where(c => c.StudentID == student.ID) //only for given student
                .AsNoTracking(); //no caching
            //Get the student name for display in view using view data
            ViewData["StudentName"] = student.FullName;


            //8. mwilliams: Courses Available (courses that current student is not enrolled in)
            //   Build a RAW SQL Query using LINQ for this demo
            string query = @"SELECT CourseID, Credits, Title, DepartmentID
                            FROM Course
                            WHERE CourseID NOT IN(SELECT DISTINCT CourseID
                            FROM Enrollment
                            WHERE StudentID = {0})";
            var courses = _context.Courses.FromSqlRaw(query, student.ID).AsNoTracking().ToList();

            //send course back to view using ViewBag
            ViewData["Courses"] = courses;
            //9. mwilliams: return view with enrollment data
            
            return View(await studentEnrollments.ToListAsync());
        }
        public async Task<IActionResult> Enroll(int? id)
        {
            if(id==null)
            {
                //missing param - 404
                return NotFound();
            }
            //Get currently logged in student
            var user = await GetCurrentUserAsync(); //which user is logged in 
            if(user==null)
            {
                //user not found - 404
                return NotFound();
            }

            //if we get this far, we have a logged in user - locate the corresponding student for this user
            var student = await _context.Students
                .Include(s => s.Enrollments)
                .AsNoTracking()
                .SingleOrDefaultAsync(s => s.Email == user.Email);

            //Send data back to view (ViewData) for form hidden field (so we know who they are)
            ViewData["StudentID"] = student.ID;

            //Retrieve the current student's current enrollment
            //(for comparison with the course they want to enroll in)
            //- Student cannot enroll twice in same course
            var studentEnrollments = new HashSet<int>(_context.Enrollments
                .Include(e => e.Course) //we need course to get CourseID
                .Where(e => e.StudentID == student.ID)//for current student
                .Select(e => e.CourseID)); //Only select the CourseID

            //Check for method parameter
            int currentCourseID;
            if( id.HasValue ) //id here is the method parameter (course id)
            {
                currentCourseID = (int)id;
            }
            else
            {
                currentCourseID = 0;
            }

            //Handle situation where student tries to enroll in same course
            if(studentEnrollments.Contains(currentCourseID))
            {
                //Same course - send back error to view as ModelState error
                ModelState.AddModelError("AlreadyEnrolled",
                    "You are already enrolled in this course!");
            }

            
            //first find the course
            var course = await 
                _context.Courses.SingleOrDefaultAsync(c => c.CourseID == id.Value);
            
            //Handle situation where student tries to enroll non-existent course
            if (course == null)
            {
                return NotFound();
            }
            
            //return view - attach the course model to it
            return View(course);
            
        }//end of Get Enroll

        //POST: Enroll
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enroll([Bind("CourseID,StudentID")] Enrollment enrollment)
        {
            //Add new enrollment object to database context
            _context.Add(enrollment);
            //Save it to database
            await _context.SaveChangesAsync();
            //return to database view
            return RedirectToAction("Index");
        }//End post enroll
        public async Task<IActionResult> UnEnroll(int? id, bool? saveChangesError=false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollments
                .Include(e => e.Course)
                .Include(e => e.Student)
                .FirstOrDefaultAsync(m => m.EnrollmentID == id);
            if (enrollment == null)
            {
                return NotFound();
            }
            
            if (enrollment.Grade != null)
            {
                ModelState.AddModelError("HashGrade",
                    "You cannot remove this class because you have a grade!");
                
                if (saveChangesError.GetValueOrDefault())
                {
                    ViewData["ErrorMessage"] =
                        "UnEnroll failed. Try again, and if the problem persists" +
                        "see you system administrator.";
                }
            }

            return View(enrollment);
        }
        //GET: UnEnroll

        //POST: UnEnroll
        [HttpPost, ActionName("UnEnroll")]
        public async Task<IActionResult> UnEnrollConfirmed(int EnrollmentID)
        {
            //we will be using a read-first approach (retrieve the particular enrollment object)
            var enrollment = await _context.Enrollments
                .AsNoTracking()
                .SingleOrDefaultAsync(e => e.EnrollmentID == EnrollmentID);
            if (enrollment == null)
            {
                return RedirectToAction(nameof(Index));
            }
            if(enrollment.Grade != null)
            {
                //Student has a grade for this enrollment - cannot delete
                ModelState.AddModelError("HasGrade", 
                    "You cannot remove this class because you have a grade!");
                return View(enrollment);
            }
            // If we get this far delete it
            try
            {
                
                _context.Enrollments.Remove(enrollment);
                await _context.SaveChangesAsync();
                //redirect user back to index page
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                return RedirectToAction(nameof(UnEnroll),
                    new { id = EnrollmentID, saveChangesError = true });
            }

        }//end of UnEnroll POST

    }//End Class
}//End namespace
       

        