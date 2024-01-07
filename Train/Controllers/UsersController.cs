using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Train.Data;
using Train.Models;
using Train.Models.Identity;
using Train.ViewModels;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Train.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public UsersController(ApplicationDbContext context,UserManager <ApplicationUser>userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [HttpGet("/user")]
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

            var totalCount = _context.Users.Count(); // Get total number of records

            var users = _context.Users
                .OrderBy(e => e.FullName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var viewModel = new UsersViewModel
            {
                Users = users,
                PageNumber = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };

            return View(viewModel);
        }


        [HttpGet]
        public IActionResult Create()
        {
            return PartialView();
        }
        [HttpPost]
        public async Task<IActionResult> Create(UserViewModel input)
        {
            // logic
            if (!ModelState.IsValid)
                return RedirectToAction("Index", new { error = "Model not valid!" });
            // save to database
            else if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = input.FullName,
                    Email = input.Email,
                    // Remove the code below to require the user to confirm their e-mail
                    EmailConfirmed = true,
                    // Custom fields next
                    FullName = input.FullName,
                };
                var result = await _userManager.CreateAsync(user, input.Password);



            }
           
            // return to list of users
            return RedirectToAction("Index", new { success = "User is created." });
        }
        [HttpGet]
        public IActionResult GetUserById(string id)
        {
            // Your edit logic here
            var user = _context.Users.SingleOrDefault(x => x.Id == id);
            // Return the Users data as JSON
            return Json(new
            {
                user.Id,
                user.FullName,
                user.Email,
               
            });
        }

        [HttpPost]
        public IActionResult Edit(ApplicationUser user)
        {
            // Your edit logic here
            // validate
            if(!ModelState.IsValid)
                return RedirectToAction("Index", new { error = "model not valid." });

            // update database 
            _context.Update(user);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // POST: Users/Delete/5
        [HttpPost]
        public IActionResult Delete(string id)
        {
            var user = _context.Users.FirstOrDefault(e => e.Id == id);

            if (user == null)
            {
                return RedirectToAction("Index", new { error = "user not found." });
            }
            _context.Users.Remove(user);
            _context.SaveChanges();
            return RedirectToAction("Index", new{success="user has been deleted" });
        }


        //Search action
        public IActionResult Search(string query)
        {
            List<ApplicationUser> users;

            if (string.IsNullOrWhiteSpace(query))
            {
                // If the query is empty, return all employees
                users = _context.Users.ToList();
            }
            else
            {
                // Perform the search based on the non-empty query
                users = _context.Users
                    .Where(e => e.FullName.Contains(query) || e.Email.Contains(query))
                    .ToList();
            }

            users = users.OrderBy(e => e.FullName)
                .Take(5)
                .ToList();

            var model = new UsersViewModel
            {
                Users = users,
                TotalCount = users.Count(),
                PageSize = 5,
                PageNumber = 1
            };

            return PartialView("_UserTablePartial", model);
        }

        //Filter
        public IActionResult Filter(string fullName, string email)
        {
            IQueryable<ApplicationUser> usersQuery = _context.Users;

            if (!string.IsNullOrWhiteSpace(fullName))
            {
                usersQuery = usersQuery.Where(usr => usr.FullName.StartsWith(fullName));
            }

            if (!string.IsNullOrWhiteSpace(email))
            {
                usersQuery = usersQuery.Where(usr => usr.Email.StartsWith(email));
            }

            var users = usersQuery.OrderBy(e => e.FullName)
                                           .Take(5)
                                           .ToList();

            var model = new UsersViewModel
            {
                Users = users,
                TotalCount = usersQuery.Count(),
                PageSize = 5,
                PageNumber = 1
            };

            return PartialView("_UserTablePartial", model);
        }



    }
    }

