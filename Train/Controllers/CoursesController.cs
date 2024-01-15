using Microsoft.AspNetCore.Mvc;
using Train.Data;
using Train.Enums;
using Train.Models;
using Train.ViewModels;
using Microsoft.AspNetCore.Identity;
using Train.Models.Identity;

namespace Train.Controllers
{
    
    public class CoursesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public CoursesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;

        }
        [HttpGet("/courses")]
        public IActionResult Index(int page = 1, int pageSize = 5)
        {
            if (HttpContext.Request.Query.TryGetValue("success", out var successValue))
            {
                // Retrieve the value of the "success" query string parameter
                string success = successValue.ToString();

                // Save the value in ViewBag to pass it to the view
                ViewBag.SuccessMessage = success;
            }
            if (HttpContext.Request.Query.TryGetValue("error", out var errorValue))
            {
                // Retrieve the value of the "success" query string parameter
                string error = errorValue.ToString();

                // Save the value in ViewBag to pass it to the view
                ViewBag.ErrorMessage = error;
            }


            var totalCount = _context.Courses.Count(); // Get total number of records

            var course = _context.Courses
                .OrderBy(e => e.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var viewModel = new CoursesViewModel
            {
                Courses = course,
                PageNumber = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };

            return View(viewModel);


        }

        [HttpGet]
        public IActionResult Create()
        {
            return PartialView();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CourseViewModel model)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Index", new { error = "Model not valid!" });

            // Get the current logged-in user
            ApplicationUser user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                var course = new Course
                {
                    Name = model.Name,
                    Description = model.Description,
                    AdattionDate = model.AdattionDate,
                    Location = model.Location,
                    Seats = model.Seats,
                    Status = model.Status,
                    UserId = user.Id
                };

                // save to database
                _context.Courses.Add(course);
                _context.SaveChanges();

                // return to list of courses
                return RedirectToAction("Index", new { success = "Course is created." });
            }

            // If user is null (not logged in), handle as needed
            return RedirectToAction("Index", new { error = "User not found!" });
        }


       


        [HttpGet]
        public IActionResult GetCoursesId(Guid id)
        {
            // Your edit logic here
            var course = _context.Courses.SingleOrDefault(x => x.Id == id);
            return Json(new
            {
                course.Id,
                course.Name,
                course.Description,
                course.Location,
                course.AdattionDate,
                course.Seats
            });
        }

        
        [HttpPost]
        public IActionResult Edit(CourseViewModel model)
        {
            // Your edit logic here
            // validate
            var course = new Course
            {
                Name = model.Name,
                Description = model.Description,
                Location = model.Location,
                Status= model.Status,
                AdattionDate= model.AdattionDate,
                //Seats = model.Seats,

            };
            // update database 
            _context.Update(course);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // POST: Courses/Push/5
        [HttpPost]
        public IActionResult Push(Guid id)
        {
            var course = _context.Courses.FirstOrDefault(e => e.Id == id);

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
        public IActionResult Delete(Guid id)
        {
            var course = _context.Courses.FirstOrDefault(e => e.Id == id);

            if (course == null)
            {
                return RedirectToAction("Index", new { error = "user not found." });
            }
            _context.Courses.Remove(course);
            _context.SaveChanges();
            return RedirectToAction("Index", new { success = "course has been deleted" });
        }


        // Search action
        public IActionResult Search(string query)
        {
            List<Course> courses;

            if (string.IsNullOrWhiteSpace(query))
            {
                // If the query is empty, return all employees
                courses = _context.Courses.ToList();
            }
            else
            {
                // Perform the search based on the non-empty query
                courses = _context.Courses
                    .Where(e => e.Name.StartsWith(query) || e.AdattionDate.ToString().StartsWith(query) 
                    || e.Description.StartsWith(query) || e.Location.StartsWith(query))
                    .ToList();
            }

            courses = courses.OrderBy(e => e.Name)
                .Take(5)
                .ToList();

            var model = new CoursesViewModel
            {
                Courses = courses,
                TotalCount = courses.Count(),
                PageSize = 5,
                PageNumber = 1
            };

            return PartialView("_CourseTablePartial", model);
        }

        //Filter
        public IActionResult Filter(string name, string description, string location, ProgramStatus status)
        {
            List<Course> courses;

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(description) 
                || string.IsNullOrWhiteSpace(location ))
            {
                // If the query is empty, return all employees
                courses = _context.Courses.ToList();
            }
            else
            {
                courses = _context.Courses
                    .Where(e => e.Name.StartsWith(name) ||
                                e.Description.StartsWith(description)
                                ||
                                e.Location.StartsWith(location)
                              
                                
                                ).ToList();
            }

            courses = courses.OrderBy(e => e.Name)
                .Take(5)
                .ToList();

            var model = new CoursesViewModel
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
