using Microsoft.AspNetCore.Mvc;
using Train.Services;

namespace Train.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly EmployeeService _employeeService;
        public EmployeesController(EmployeeService employeeService)
        {
            _employeeService = employeeService;
        }
        [HttpGet("/employees")]
        public IActionResult Index()
        {
            var employees = _employeeService.GetEmployeeList();
            return View(employees);
        }
    }
}
