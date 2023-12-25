using Microsoft.AspNetCore.Mvc;
using Train.Data;
using Train.Models;
using Train.Services;

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
        public IActionResult Index()
        {
            var employees = _context.Employees.ToList();
            return View(employees);
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

        public IActionResult Delete(int id)
        {
            // Your delete logic here
            return View();
        }
    }
}
