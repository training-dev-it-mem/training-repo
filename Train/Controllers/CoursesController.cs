using Microsoft.AspNetCore.Mvc;
using Train.Data;
using Train.Enums;
using Train.Models;
using Train.ViewModels;
using Microsoft.AspNetCore.Identity;
using Train.Models.Identity;
using Microsoft.EntityFrameworkCore;

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

            var courses = _context.Courses.ToList();

            return View(viewModel);


        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = new CourseViewModel();
            return PartialView(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CourseViewModel model)
        {
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
        public IActionResult Details(string courseId, int page = 1, int pageSize = 5)
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
            var course = _context.Courses
                .Include(c => c.Batches)
                .Where(c => c.Id == courseId).FirstOrDefault();

            var user = _context.Users.FirstOrDefault(u => u.Id == course.UserId);
            var userName = user != null ? user.Name : "لا يوجد";
            var viewModel = new CourseDetailsViewModel
            {
                Id = course.Id,
                Name = course.Name,
                Description = course.Description,
                Location = course.Location,
                AdattionDate = course.AdattionDate,
                Seats = course.Seats,
                Status = course.Status,
                CreatedBy = userName,
                batches = course.Batches.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
                PageNumber = page,
                PageSize = 5,
                TotalCount = course.Batches.Count()
            };
            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Edit(string id)
        {
            var course = _context.Courses.Find(id);
            var model = new CourseViewModel
            {
                Id = course.Id,
                Name = course.Name,
                Description = course.Description,
                Location = course.Location,
                AdattionDate = course.AdattionDate,
                Seats = course.Seats
            };
            return PartialView(model);
        }

        [HttpPost]
        public IActionResult Edit(CourseViewModel model)
        {
            if(!ModelState.IsValid)
                return RedirectToAction("Details", new
                { courseId = model.Id, page = 1, pageSize = 5, error = "Cousre not Updated." });
            var course = _context.Courses.Find(model.Id);
            course.Name = model.Name;
            course.Description = model.Description;
            course.Location = model.Location;
            course.AdattionDate = model.AdattionDate;
            course.Seats = model.Seats;

            _context.Courses.Update(course);
            _context.SaveChanges();

            return RedirectToAction("Details", new
            { courseId = model.Id, page = 1, pageSize = 5, success="Cousre has been Updated." });
        }
        
        // POST: Courses/Push/5
        [HttpPost]
        public IActionResult Push(string id)
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
        public IActionResult Delete(string id)
        {
            var course = _context.Courses.FirstOrDefault(e => e.Id == id);

            if (course == null)
            {
                return RedirectToAction("Index", new { error = "course not found." });
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
        public IActionResult Filter(string name,string additionDate , ProgramStatus status)
        {
            IQueryable<Course> coursesQuery = _context.Courses;

            if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(additionDate) && status == null)
            {
                // If all filters are empty, return all courses
                coursesQuery = _context.Courses;
            }
           
            // Apply filters if provided
            if (!string.IsNullOrWhiteSpace(name))
            {
                coursesQuery = coursesQuery.Where(e => e.Name.StartsWith(name));
            }

            if (!string.IsNullOrWhiteSpace(additionDate))
            {
                var arrDates = additionDate.Split("-");

                DateTime startDate = DateTime.Parse(arrDates[0]);
                DateTime endDate = DateTime.Parse(arrDates[1]);

                coursesQuery = coursesQuery
                    .Where(e => e.AdattionDate.Date >= startDate.Date
                    && e.AdattionDate <= endDate.Date);
            }

            if (status !=null)
            {
                coursesQuery = coursesQuery.Where(e => e.Status == status);
            }
            

            List<Course> courses = coursesQuery.OrderBy(e => e.Name)
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
