using Microsoft.AspNetCore.Mvc;
using Train.Data;
using Train.Models;
using Train.ModelViews;

namespace Train.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _context;
        public EmployeesController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet("/employees")]
        public IActionResult Index(int page = 1, int pageSize = 5)
        {
            var totalCount = _context.Employees.Count(); // Get total number of records

            var employees = _context.Employees
                .OrderBy(e => e.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var viewModel = new EmployeeViewModel
            {
                Employees = employees,
                PageNumber = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Create(Employee employee) {
            // logic

            // save to database
            _context.Employees.Add(employee);
            _context.SaveChanges();
            // return to list of employees
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult GetEmployeeById(int id)
        {
            // Your edit logic here
         var employee = _context.Employees.SingleOrDefault(x => x.Id == id);
            // Return the employee data as JSON
            return Json(new
            {
                employee.Id,
                employee.Name,
                employee.Email,
            });
        }
        [HttpPost]
        public IActionResult Edit(Employee employee)
        {
            // Your edit logic here
            // validate

            // update database 
            _context.Update(employee);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        // POST: Employees/Delete/5
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var employee = _context.Employees.FirstOrDefault(e => e.Id == id);

            if (employee == null)
            {
                return NotFound();
            }
            _context.Employees.Remove(employee);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        // Search action
        public IActionResult Search(string query)
        {
            List<Employee> employees;

            if (string.IsNullOrWhiteSpace(query))
            {
                // If the query is empty, return all employees
                employees = _context.Employees.ToList();
            }
            else
            {
                // Perform the search based on the non-empty query
                employees = _context.Employees
                    .Where(e => e.Name.Contains(query) || e.Email.Contains(query))
                    .ToList();
            }

            employees = employees.OrderBy(e => e.Name)
                .Take(5)
                .ToList();

            var model = new EmployeeViewModel
            {
                Employees = employees,
                TotalCount = employees.Count(),
                PageSize = 5,
                PageNumber = 1
            };

            return PartialView("_EmployeeTablePartial", model);
        }
    }
}
