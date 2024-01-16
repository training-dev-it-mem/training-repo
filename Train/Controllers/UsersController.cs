using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Train.Data;
using Train.Models.Identity;
using Train.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Train.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        public UsersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
        }
        [HttpGet("/users")]
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
                .OrderBy(e => e.Name)
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
            var departments = _context.Departments.ToList();
            var departmentList = departments.Select(d => new SelectListItem
            {
                Value = d.Id,
                Text = d.Name
            }).ToList();

            var roles = _context.Roles
                            .Where(role => role.Name != "Admin")
                            .ToList();

            var rolesList = roles.Select(r => new SelectListItem
            {
                Value = r.Id,
                Text = r.Name
            }).ToList();

            var viewModel = new UserViewModel
            {
                DepartmentList = departmentList,
                RolesList = rolesList
            };

            return PartialView(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index", new { error = "Model not valid!" });
            }
            // Create the user without a password
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                EmailConfirmed = false, // Email is not yet confirmed
                Name = model.Name,
            };
            var result = await _userManager.CreateAsync(user);

            if (result.Succeeded)
            {
                // Generate email confirmation token
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                // Construct the URL for the password setup page
                var passwordSetupUrl = Url.Action("SetPassword", "Account", new { userId = user.Id, token = token }, Request.Scheme);

                // Send email to the user with the password setup link
                await _emailSender.SendEmailAsync(user.Email, "Set Your Password",
                    $"Please set your password by clicking <a href='{passwordSetupUrl}'>here</a>.");

                return RedirectToAction("Index", new { success = "User has been created. Check your email to set your password." });
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                // Serialize the errors into a string
                string serializedErrors = string.Join("; ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));

                return RedirectToAction("Index", new { error = serializedErrors });
            }
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
                user.Name,
                user.Email,

            });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            //First Fetch the User Details by UserId
            var user = await _userManager.FindByIdAsync(id);
            //Check if User Exists in the Database
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("NotFound");
            }
            // GetClaimsAsync retunrs the list of user Claims
            var userClaims = await _userManager.GetClaimsAsync(user);
            // GetRolesAsync returns the list of user Roles
            var userRoles = await _userManager.GetRolesAsync(user);
            //Store all the information in the EditUserViewModel instance
            var model = new EditUserViewModel
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Roles = userRoles
            };
            return PartialView("_Edit", model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                var error = $"User with Id = {model.Id} cannot be found.";
                return RedirectToAction("Index", new { error });
            }
            else
            {
                //Populate the user instance with the data from EditUserViewModel
                user.Name = model.Name;
                user.Email = model.Email;
                user.UserName = model.Email;
                //UpdateAsync Method will update the user data in the AspNetUsers Identity table
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    //Once user data updated redirect to the ListUsers view
                    return RedirectToAction("Index", new { success = "User has been updated." });
                }
                else
                {
                    //In case any error, stay in the same view and show the model validation error
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    // Serialize the errors into a string
                    string serializedErrors = string.Join("; ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage));

                    return RedirectToAction("Index", new { error = serializedErrors });
                }
            }
        }

        // POST: Users/Delete/5
        [HttpPost]
        public IActionResult Delete(string id)
        {
            var user = _context.Users.FirstOrDefault(e => e.Id == id);

            if (user == null)
            {
                return RedirectToAction("Index", new { error = "User not found." });
            }
            _context.Users.Remove(user);
            _context.SaveChanges();
            return RedirectToAction("Index", new { success = "User has been deleted." });
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
                    .Where(e => e.Name.Contains(query) || e.Email.Contains(query))
                    .ToList();
            }

            users = users.OrderBy(e => e.Name)
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
                usersQuery = usersQuery.Where(usr => usr.Name.StartsWith(fullName));
            }

            if (!string.IsNullOrWhiteSpace(email))
            {
                usersQuery = usersQuery.Where(usr => usr.Email.StartsWith(email));
            }

            var users = usersQuery.OrderBy(e => e.Name)
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

        // Get: Users/ChangePassword
        [HttpGet]
        public IActionResult UpdatePassword()
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

            return View();
        }
        // Post: Users/UpdatePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    return NotFound();
                }

                var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

                if (result.Succeeded)
                {
                    // Password changed successfully
                    return RedirectToAction("UpdatePassword", new { success = "Password has been updated." });
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    string serializedErrors = string.Join("; ", ModelState.Values
                       .SelectMany(v => v.Errors)
                       .Select(e => e.ErrorMessage));

                    // If ModelState is not valid, return the view with validation errors
                    return RedirectToAction("UpdatePassword", new { error = serializedErrors });
                }
            }
            return View(model);
        }
    }
}

