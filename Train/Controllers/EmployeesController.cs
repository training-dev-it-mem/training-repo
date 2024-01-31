using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Printing;
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
            var employeesNotAssignedToThisBatch = GetNotAssignEmployeesToBatch(batchId);

            var model = new AssignEmployeesViewModel
            {
                BatchId = batchId,
                Employees = employeesNotAssignedToThisBatch.Take(5).ToList(),
                TotalCount = employeesNotAssignedToThisBatch.Count()
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult Filter(string batchId, int page = 1, int pageSize = 5, string searchTerm = "")
        {
            // Your logic to filter and paginate the data
            var filteredEmployees = GetNotAssignEmployeesToBatch(batchId);

            // Apply search filter if a search term is provided
            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower(); // Convert to lowercase for case-insensitive comparison

                filteredEmployees = filteredEmployees
                    .Where(employee =>
                        employee.Name.ToLower().Contains(searchTerm) ||
                        employee.Email.ToLower().Contains(searchTerm) ||
                        employee.Department.Name.ToLower().Contains(searchTerm))
                    .ToList();
            }

            var model = new AssignEmployeesViewModel
            {
                Employees = filteredEmployees.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
                PageNumber = page,
                PageSize = pageSize,  // Make sure to include PageSize in the model
                TotalCount = filteredEmployees.Count()
            };

            // Return JSON data
            return Json(model);
        }

        [HttpPost]
        public IActionResult Assign(string batchId, string selectedEmployeeIds)
        {
            // Split the comma-separated string into a list of strings
            List<string> selectedIdsList = selectedEmployeeIds.Split(',').ToList();

            foreach (var selectedEmployeeId in selectedIdsList)
            {
                var batchEmployee = new BatchEmployees
                {
                    BatchId = batchId,
                    EmployeeId = selectedEmployeeId
                };
                _context.BatchEmployees.Add(batchEmployee);
            }
            _context.SaveChanges();
            // Redirect to a new page after successful assignment
            return RedirectToAction("Details", "Batch", new { batchId });
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
        // Your helper method to filter employees based on batchId and selectedEmployeeIds
        private List<ApplicationUser> GetNotAssignEmployeesToBatch(string batchId)
        {
            var batchEmployees = _context.BatchEmployees.Where(b => b.BatchId == batchId).Select(be => be.Employee).ToList();
            var employeesNotAssignedToThisBatch = _context.Users.Include(user => user.Department)
                .Where(user => !batchEmployees.Contains(user)).ToList();
           
            return employeesNotAssignedToThisBatch;
        }
    }
}
