using Microsoft.AspNetCore.Mvc;
using Train.Data;
using Train.Models;
using Train.Services;

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
        public IActionResult Index()
        {
            var managers = _context.Managers.ToList();
            return View(managers);

            
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
    }
}
