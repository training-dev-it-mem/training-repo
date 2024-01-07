using Microsoft.AspNetCore.Mvc;
using Train.Data;
using Train.Models;
using Train.ViewModels;

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
        public IActionResult Create()
        {
            return PartialView();
        }

        [HttpPost]
        public IActionResult Create(Managers managers)
        {
            // logic
            if (!ModelState.IsValid)
                return RedirectToAction("Index", new { error = "Model not valid!" });
            // save to database
            _context.Managers.Add(managers);
            _context.SaveChanges();
            // return to list of employees
            return RedirectToAction("Index", new { success = "Manager has been created." });
        }

        [HttpGet]
        public IActionResult GetManagersById(int id)
        {
            // Your edit logic here
            var manager = _context.Managers.SingleOrDefault(x => x.Id == id);
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
                return RedirectToAction("Index", new { error = "user not found." });
            }
            _context.Managers.Remove(manager);
            _context.SaveChanges();
            return RedirectToAction("Index", new { success = "Manager has been deleted" });
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
            IQueryable<Managers> managersQuery = _context.Managers;

            if (!string.IsNullOrWhiteSpace(name))
            {
                managersQuery = managersQuery.Where(mng => mng.Name.StartsWith(name));
            }

            if (!string.IsNullOrWhiteSpace(email))
            {
                managersQuery = managersQuery.Where(mng => mng.Email.StartsWith(email));
            }

            var managers = managersQuery.OrderBy(e => e.Name)
                                           .Take(5)
                                           .ToList();

            var model = new ManagerViewModel
            {
                Managers = managers,
                TotalCount = managersQuery.Count(),
                PageSize = 5,
                PageNumber = 1
            };

            return PartialView("_ManagerTablePartial", model);
        }


    }
}


