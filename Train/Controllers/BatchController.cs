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
        public IActionResult Create()
        {
            // Assume you retrieve a list of managers from your data source
            var courses = _context.Courses.ToList(); // Replace with your Course retrieval logic

            // Populate ViewBag.Managers with the list of managers
            ViewBag.Programs = courses;

            return PartialView();
        }

        [HttpPost]
        public IActionResult Create(BatchViewModel model)
        {
            // logic
            if (!ModelState.IsValid)
                return RedirectToAction("Index", new { error = "Model not valid!" });

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
            return RedirectToAction("Index", new { success = "batch has been created." });
        }
        [HttpGet]
        public IActionResult GetBatchId(Guid id)
        {
            // Your edit logic here
            var batch = _context.Batches.SingleOrDefault(x => x.Id == id);
            return Json(new
            {
                batch.Id,
                batch.StartDate,
                batch.EndDate,
                batch.StartTime,
                batch.EndTime
               
            });
        }


        [HttpPost]
        public IActionResult Edit(Batch batch)
        {
            // Your edit logic here
            // validate

            // update database 
            _context.Update(batch);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // POST: Batch/Delete/5
        [HttpPost]
        public IActionResult Delete(Guid id)
        {
            var batch = _context.Batches.FirstOrDefault(e => e.Id == id);

            if (batch == null)
            {
                return RedirectToAction("Index", new { error = "user not found." });
            }
            _context.Batches.Remove(batch);
            _context.SaveChanges();
            return RedirectToAction("Index", new { success = "Batch has been deleted" });
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
        public IActionResult Filter(DateTime startdate, DateTime enddate)
        {
            IQueryable<Batch> batchQuery = _context.Batches;

            if (startdate != default(DateTime))
            {
                batchQuery = batchQuery.Where(btc => btc.StartDate >= startdate);
            }

            if (enddate != default(DateTime))
            {
                batchQuery = batchQuery.Where(btc => btc.EndDate <= enddate);
            }

            var batches = batchQuery.OrderBy(e => e.StartDate)
                                           .Take(5)
                                           .ToList();

            var model = new BatchsViewModel
            {
                batches = batches,
                TotalCount = batchQuery.Count(),
                PageSize = 5,
                PageNumber = 1
            };

            return PartialView("_BatchTablePartial", model);
        }




    }

}
