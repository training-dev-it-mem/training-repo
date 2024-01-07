using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Train.Data;
using Train.Models;
using Train.ViewModels;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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


            var totalCount = _context.Employees.Count(); // Get total number of records

            var employees = _context.Employees
                .OrderBy(e => e.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var viewModel = new EmployeesViewModel
            {
                Employees = employees,
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
            var managers = _context.Managers.ToList(); // Replace with your manager retrieval logic

            // Populate ViewBag.Managers with the list of managers
            ViewBag.Managers = managers;

            return PartialView();
        }

        [HttpPost]
        public IActionResult Create(EmployeeViewModel model) {

            if (!ModelState.IsValid)
                return RedirectToAction("Index", new {error = "Model not valid!"});

            var employee = new Employee
            {
                Name = model.Name,
                Email = model.Email,
                ManagerId = model.ManagerId
            };
            // save to database
            _context.Employees.Add(employee);
            _context.SaveChanges();
            // return to list of employees
            return RedirectToAction("Index", new { success = "Employee created."});
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
                return RedirectToAction("Index", new { error = "user not found." });
            }
            _context.Employees.Remove(employee);
            _context.SaveChanges();
            return RedirectToAction("Index", new { success = "Employee has been deleted" });
        }
        //Search action
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

            var model = new EmployeesViewModel
            {
                Employees = employees,
                TotalCount = employees.Count(),
                PageSize = 5,
                PageNumber = 1
            };

            return PartialView("_EmployeeTablePartial", model);
        }

        //Filter
        public IActionResult Filter(string name, string email)
        {
            IQueryable<Employee> employeesQuery = _context.Employees;

            if (!string.IsNullOrWhiteSpace(name))
            {
                employeesQuery = employeesQuery.Where(emp => emp.Name.StartsWith(name));
            }

            if (!string.IsNullOrWhiteSpace(email))
            {
                employeesQuery = employeesQuery.Where(emp => emp.Email.StartsWith(email));
            }

            var employees = employeesQuery.OrderBy(e => e.Name)
                                           .Take(5)
                                           .ToList();

            var model = new EmployeesViewModel
            {
                Employees = employees,
                TotalCount = employeesQuery.Count(),
                PageSize = 5,
                PageNumber = 1
            };

            return PartialView("_EmployeeTablePartial", model);
        }

    }
}
