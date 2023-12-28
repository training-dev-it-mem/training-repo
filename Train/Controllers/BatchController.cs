using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Train.Data;
using Train.Models;

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
        public IActionResult Index()
        {
            var batches = _context.Batches.ToList();
            return View(batches);


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
    }

}
