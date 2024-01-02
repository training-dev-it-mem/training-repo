using Microsoft.AspNetCore.Mvc;
using Train.Data;
using Train.Models;
using Train.ModelViews;

namespace Train.Controllers
{
    public class ManagersController : Controller
    {

   
        private readonly ApplicationDbContext _context;
        public ManagersController(ApplicationDbContext context)
        {
            _context = context;
            
        }
        [HttpGet("/managers")]
        public IActionResult Index(int page = 1, int pageSize = 5)
        {
            var totalCount = _context.Managers.Count(); // Get total number of records

            var manager = _context.Managers
                .OrderBy(e => e.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var viewModel = new ManagerViewModel
            {
                Managers = manager,
                PageNumber = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };

            return View(viewModel);


        }
        [HttpPost]
        public IActionResult Create(Managers manager)
        {
            // logic

            // save to database
            _context.Managers.Add(manager);
            _context.SaveChanges();
            // return to list of employees
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult GetManagersById(int id)
        {
            // Your edit logic here
            var manager = _context.Managers.SingleOrDefault(x => x.Id == id);
            if (manager == null)
            {
                return NotFound();
            }
            // Return the employee data as JSON
            return Json(new
            {
                manager.Id,
                manager.Name,
                manager.Email,
            });
        }

        [HttpPost]
        public IActionResult Edit(Managers manager)
        {
            // Your edit logic here
            // validate

            // update database 
            _context.Update(manager);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // POST: Manager/Delete/5
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var manager = _context.Managers.FirstOrDefault(e => e.Id == id);

            if (manager == null)
            {
                return NotFound();
            }
            _context.Managers.Remove(manager);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // Search action
        public IActionResult Search(string query)
        {
            List<Managers> managers;

            if (string.IsNullOrWhiteSpace(query))
            {
                // If the query is empty, return all employees
                managers = _context.Managers.ToList();
            }
            else
            {
                // Perform the search based on the non-empty query
                managers = _context.Managers
                    .Where(e => e.Name.Contains(query) || e.Email.Contains(query))
                    .ToList();
            }

            managers = managers.OrderBy(e => e.Name)
                .Take(5)
                .ToList();

            var model = new ManagerViewModel
            {
                Managers = managers,
                TotalCount = managers.Count(),
                PageSize = 5,
                PageNumber = 1
            };

            return PartialView("_ManagerTablePartial", model);
        }

        //Filter
        public IActionResult Filter(string name, string email)
        {
            List<Managers> managers;

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email))
            {
                // If the query is empty, return all employees
                managers = _context.Managers.ToList();
            }
            else
            {
                managers = _context.Managers
                    .Where(e => e.Name.Contains(name) &&
                                e.Email.Contains(email))
                    .ToList();
            }

            managers = managers.OrderBy(e => e.Name)
                .Take(5)
                .ToList();

            var model = new ManagerViewModel
            {
                Managers = managers,
                TotalCount = managers.Count(),
                PageSize = 5,
                PageNumber = 1
            };

            return PartialView("_ManagerTablePartial", model);
        }


    }
}


