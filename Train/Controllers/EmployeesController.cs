using Microsoft.AspNetCore.Mvc;
using Train.Data;
using Train.Models;
using Train.Services;

namespace Train.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly EmployeeService _employeeService;
        private readonly ApplicationDbContext _context;
        public EmployeesController(ApplicationDbContext context, EmployeeService employeeService)
        {
            _context = context;
            _employeeService = employeeService;
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
    }
}
