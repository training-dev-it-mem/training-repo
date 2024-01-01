using Microsoft.AspNetCore.Mvc;
using Train.Data;
using Train.Models;
using Train.ModelViews;


namespace Train.Controllers
{
    
    public class CoursesController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CoursesController(ApplicationDbContext context)
        {
            _context = context;

        }
        [HttpGet("/courses")]
        public IActionResult Index(int page = 1, int pageSize = 5)
        {
            var totalCount = _context.Programs.Count(); // Get total number of records

            var course = _context.Programs
                .OrderBy(e => e.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var viewModel = new CourseViewModel
            {
                Courses = course,
                PageNumber = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };

            return View(viewModel);


        }
        [HttpPost]
        public IActionResult Create(Course courses)
        {
            // logic
            //courses.Status = Enums.ProgramStatus.Approve;
            // save to database
            _context.Programs.Add(courses);
            _context.SaveChanges();
            // return to list of Courses
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult GetCoursesId(int id)
        {
            // Your edit logic here
            var course = _context.Programs.SingleOrDefault(x => x.Id == id);
            return Json(new
            {
                course.Id,
                course.Name,
                course.Description,
                course.Location,
                course.AdattionDate
            });
        }

        
        [HttpPost]
        public IActionResult Edit(Course course)
        {
            // Your edit logic here
            // validate

            // update database 
            _context.Update(course);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // POST: Courses/Push/5
        [HttpPost]
        public IActionResult Push(int id)
        {
            var course = _context.Programs.FirstOrDefault(e => e.Id == id);

            if (course == null)
            {
                return NotFound();
            }
            course.Status = Enums.ProgramStatus.Sent;
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // POST: Courses/Delete/5
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var course = _context.Programs.FirstOrDefault(e => e.Id == id);

            if (course == null)
            {
                return NotFound();
            }
            _context.Programs.Remove(course);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }


        // Search action
        public IActionResult Search(string query)
        {
            List<Course> courses;

            if (string.IsNullOrWhiteSpace(query))
            {
                // If the query is empty, return all employees
                courses = _context.Programs.ToList();
            }
            else
            {
                // Perform the search based on the non-empty query
                courses = _context.Programs
                    .Where(e => e.Name.Contains(query) || e.AdattionDate.ToString().Contains(query) 
                    || e.Description.Contains(query) || e.Location.Contains(query))
                    .ToList();
            }

            courses = courses.OrderBy(e => e.Name)
                .Take(5)
                .ToList();

            var model = new CourseViewModel
            {
                Courses = courses,
                TotalCount = courses.Count(),
                PageSize = 5,
                PageNumber = 1
            };

            return PartialView("_CourseTablePartial", model);
        }



    }
}
