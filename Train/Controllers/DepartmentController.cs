using Microsoft.AspNetCore.Mvc;
using Train.Data;
using Train.Models;

namespace Train.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly ApplicationDbContext _context;
        public DepartmentController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult GetDepartments()
        {
            // Replace this with your logic to fetch the data from the database or any other source
            var departments = _context.Departments.ToList();

            // Transform the departments into a format suitable for the dropdown
            var dropdownData = departments.Select(d => new
            {
                value = d.Id,
                text = d.Name
            });

            return Json(dropdownData);
        }

        [HttpPost]
        public IActionResult Create([FromBody] string departmentName)
        {
            try
            {
                if (string.IsNullOrEmpty(departmentName))
                {
                    return BadRequest("Department name cannot be empty.");
                }

                // Check if a department with the same name already exists
                if (_context.Departments.Any(d => d.Name == departmentName))
                {
                    return Conflict("Department with the same name already exists.");
                }

                // Create a new department
                var newDepartment = new Department
                {
                    Name = departmentName
                };

                // Add the new department to the database
                _context.Departments.Add(newDepartment);
                _context.SaveChanges();

                // Return the newly created department in the desired format
                var dropdownData = new
                {
                    value = newDepartment.Id,
                    text = newDepartment.Name
                };

                return Json(dropdownData);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it according to your needs
                return StatusCode(500, "Internal Server Error");
            }
        }

    }
}
