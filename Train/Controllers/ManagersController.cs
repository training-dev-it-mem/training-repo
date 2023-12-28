﻿using Microsoft.AspNetCore.Mvc;
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

        [HttpGet]
        public IActionResult GetManagersById(int id)
        {
            // Your edit logic here
            var manager = _context.Managers.SingleOrDefault(x => x.Id == id);
            if (manager == null)
            {
                return NotFound();
            }
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

        public IActionResult Delete(int id)
        {
            // Your delete logic here
            return View();
        }
    }
}


