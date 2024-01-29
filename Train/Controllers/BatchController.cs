using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Train.Data;
using Train.Models;
using Train.ViewModels;


namespace Train.Controllers
{
  
    public class BatchController : Controller
    {
        private readonly ApplicationDbContext _context;
        public BatchController(ApplicationDbContext context)
        {
            _context = context;

        }
        [HttpGet("/batch")]
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
            var totalCount = _context.Batches.Count(); // Get total number of records

            var batch = _context.Batches
                .OrderBy(e => e.StartDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var viewModel = new BatchsViewModel
            {
                batches = batch,
                PageNumber = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };

            return View(viewModel);


        }

        [HttpGet]
        public IActionResult Create(string courseId)
        {
            var batchViewModel = new BatchViewModel { CourseId=courseId};
            return PartialView(batchViewModel);
        }

        [HttpPost]
        public IActionResult Create(BatchViewModel model)
        {
            // logic
            if (!ModelState.IsValid)
                return RedirectToAction("Details","Courses", new { error = "Model not valid!" });

            var batch = new Batch
            {
               StartDate= model.StartDate,
               EndDate= model.EndDate,
               StartTime= model.StartTime,
               EndTime= model.EndTime,
                CourseId = model.CourseId
            };
            // save to database
            _context.Batches.Add(batch);
            _context.SaveChanges();
            // return to list of batches
            return RedirectToAction("Details","Courses", new {courseId=batch.CourseId, success = "batch has been created." });
        }
        
        public IActionResult Details(string batchId)
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
            var batch = _context.Batches
                .Include(ub => ub.Employees)
                .ThenInclude(e=>e.Employee)
                .Where(c => c.Id == batchId)
                .FirstOrDefault();

            var employees = batch.Employees.Select(e => e.Employee).ToList();

            var viewModel = new BatchDetailsViewModel
            {
                Id = batch.Id,
                StartDate = batch.StartDate,
                EndDate = batch.EndDate,
                StartTime = batch.StartTime,
                EndTime = batch.EndTime,
                Employees = employees,
                PageNumber = 1,
                PageSize = 5,
                TotalCount = employees.Count()
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Edit(string id)
        {
            var batch = _context.Batches.Find(id);
            var model = new BatchViewModel
            {
                Id = batch.Id,
                StartDate= batch.StartDate,
                EndDate= batch.EndDate,
                StartTime= batch.StartTime,
                EndTime= batch.EndTime,
                CourseId= batch.CourseId
            };
            return PartialView(model);
        }

        [HttpPost]
        public IActionResult Edit(BatchViewModel model)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Details", new
                { batchId = model.Id, page = 1, pageSize = 5, error = "Batch not Updated." });

            var batch = _context.Batches.Find(model.Id);

            batch.StartDate = model.StartDate;
            batch.EndDate = model.EndDate;
            batch.StartTime = model.StartTime;
            batch.EndTime = model.EndTime;

            _context.Batches.Update(batch);
            _context.SaveChanges();

            return RedirectToAction("Details", new
            { batchId = model.Id, page = 1, pageSize = 5, success = "Batch has been Updated." });
        }

        // POST: Batch/Delete/5
        [HttpPost]
        public IActionResult Delete(string id)
        {
            var batch = _context.Batches.FirstOrDefault(e => e.Id == id);

            if (batch == null)
            {
                return RedirectToAction("Index", new { error = "user not found." });
            }
            var courseId=batch.CourseId;
            _context.Batches.Remove(batch);
            _context.SaveChanges();
            return RedirectToAction("Details", "Courses", new { courseId });
        }

        // Search action
        public IActionResult Search(string query)
        {
            List<Batch> batches;

            if (string.IsNullOrWhiteSpace(query))
            {
                // If the query is empty, return all employees
                batches = _context.Batches.ToList();
            }
            else
            {
                
                // Perform the search based on the non-empty query
                batches = _context.Batches
                    .Where(e => e.StartDate.ToString().Contains(query) || e.EndDate.ToString().Contains(query)||
                    e.StartTime.ToString().Contains(query)||e.EndTime.ToString().Contains(query))
                    .ToList();
            }

            batches = batches.OrderBy(e => e.StartDate)
                .Take(5)
                .ToList();

            var model = new BatchsViewModel
            {
                batches = batches,
                TotalCount = batches.Count(),
                PageSize = 5,
                PageNumber = 1
            };

            return PartialView("_BatchTablePartial", model);
        }

        //Filter
        public IActionResult Filter(string courseId, string dateRange,int page = 1, int pageSize = 5)
        {
            var course = _context.Courses
                .Include(c=>c.Batches)
                .Where(c=> c.Id==courseId)
                .FirstOrDefault();

            var arrDates = dateRange.Split("-");

            DateTime startDate = DateTime.Parse(arrDates[0]);
            DateTime endDate = DateTime.Parse(arrDates[1]);

            var batches = course.Batches
                .Where(b => b.StartDate.Date >= startDate.Date && b.StartDate.Date <= endDate.Date
                && b.EndDate.Date >= startDate.Date
                && b.EndDate.Date <= endDate.Date)
                .Skip((page-1) * pageSize)
                .Take(pageSize)
                .ToList();

            var model = new CourseDetailsViewModel
            {
                batches = batches,
                TotalCount = batches.Count(),
                PageSize = pageSize,
                PageNumber = page
            };

            return PartialView("_BatchTablePartial", model);
        }


    }

}
