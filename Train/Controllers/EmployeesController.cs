using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Train.Data;
using Train.Models;
using Train.Models.Identity;
using Train.ViewModels;

namespace Train.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _context;
        public EmployeesController(ApplicationDbContext context)
        {
            _context = context;

        }
        [HttpGet]
        public IActionResult Assign(string batchId)
        {
            // Batch employees
            var batchEmployees = _context.BatchEmployees.Where(b=> b.BatchId == batchId).Select(be=>be.Employee).ToList();
            var employeesNotAssigntoThisBatch = _context.Users.Include(user=>user.Department)
                .Where(user => !batchEmployees.Contains(user)).ToList();
            var model = new BatchEmployeesViewModel 
            {
                BatchId=batchId,
                Employees= employeesNotAssigntoThisBatch
            };
            return PartialView(model);
        }
        [HttpPost]
        public IActionResult Assign(string batchId, string[] employeeIds)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Details", "Batch", new { batchId, error = "not valid!" });

            foreach (var employeeId in employeeIds)
            {
                var batchEmployee = new BatchEmployees
                {
                    BatchId = batchId,
                    EmployeeId = employeeId
                };
                _context.BatchEmployees.Add(batchEmployee);
            }
            _context.SaveChanges();

            return RedirectToAction("Details", "Batch", new { batchId, success = "Employees Assigned" });
        }

        public IActionResult Unassgin(string employeeId, string batchId)
        {
            var batchemployee = _context.BatchEmployees
                .Where(be => be.EmployeeId == employeeId && be.BatchId == batchId)
                .FirstOrDefault();

            if (batchemployee == null)
                return RedirectToAction("Details", "Batch", new { batchId, error = "Employee batch not found" });

            _context.BatchEmployees.Remove(batchemployee);
            _context.SaveChanges();

            return RedirectToAction("Details", "Batch", new { batchId, success = "Employee UnAssigned"});        
        }

        public IActionResult Filter(string empname,string empemail, string empdepartment ) 
        {
            List<BatchEmployees> batchEmpolyees;
            if (string.IsNullOrWhiteSpace(empname) || string.IsNullOrWhiteSpace(empemail)
                || string.IsNullOrWhiteSpace(empdepartment))
            {
                // If the query is empty, return all employees
                batchEmpolyees = _context.BatchEmployees.ToList();
            }
            else
            {
                batchEmpolyees = _context.BatchEmployees
                    .Where(e => e.Employee.Name.StartsWith(empname) ||
                                e.Employee.Email.StartsWith(empemail)
                                
                                ).ToList();
            }
            var employees = batchEmpolyees.Select(e => e.Employee).ToList();
            employees = employees.OrderBy(e => e.Name)
                .Take(5)
                .ToList();

            var model = new BatchEmployeesViewModel
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
