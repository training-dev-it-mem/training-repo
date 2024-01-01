using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Train.Data;
using Train.Models;
using Train.ModelViews;


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
            var totalCount = _context.Batches.Count(); // Get total number of records

            var batch = _context.Batches
                .OrderBy(e => e.StartDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var viewModel = new BatchViewModel
            {
                batches = batch,
                PageNumber = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };

            return View(viewModel);


        }
        [HttpPost]
        public IActionResult Create(Batch batch)
        {
            // logic

            // save to database
            _context.Batches.Add(batch);
            _context.SaveChanges();
            // return to list of batches
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult GetBatchId(int id)
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
        public IActionResult Delete(int id)
        {
            var batch = _context.Batches.FirstOrDefault(e => e.Id == id);

            if (batch == null)
            {
                return NotFound();
            }
            _context.Batches.Remove(batch);
            _context.SaveChanges();
            return RedirectToAction("Index");
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

            var model = new BatchViewModel
            {
                batches = batches,
                TotalCount = batches.Count(),
                PageSize = 5,
                PageNumber = 1
            };

            return PartialView("_BatchTablePartial", model);
        }


    }

}
