using Microsoft.AspNetCore.Mvc;
using Train.Data;
using Train.Models;

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
        public IActionResult Index()
        {
            var courses = _context.Programs.ToList();
            return View(courses);


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
        public IActionResult Delete(int id)
        {
            // Your delete logic here
            return View();
        }

    }
}
