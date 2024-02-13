using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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
                .Include(u => u.Department)
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

            var viewModel = new CreateUserViewModel
            {
                DepartmentList = departmentList,
            };

            return PartialView(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserViewModel model)
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
                Name = model.Name,
                DepartmentId= model.DepartmentId
            };

            var result = await _userManager.CreateAsync(user);

            if (result.Succeeded)
            {
                try
                {
                    // Generate email confirmation token
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    var callbackUrl = Url.Page("/Account/ConfirmEmail", null, new { area = "Identity", userId = user.Id, code }, Request.Scheme);

                    // Separate HTML message
                    var htmlMessage = GenerateConfirmationEmail(callbackUrl);

                    // Send email to the user with the password setup link
                    await _emailSender.SendEmailAsync(user.Email, "Confirm your email", htmlMessage);

                    return RedirectToAction("Index", new { success = "User has been created. Email sent successfully." });
                }
                catch (Exception ex)
                {
                    // An error occurred while sending the email, handle the exception
                    Console.WriteLine($"Error sending email: {ex.Message}");
                    // Log the error or notify administrators

                    return RedirectToAction("Index", new { success = "User has been created. Email sending failed." });
                }
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
        public async Task<IActionResult> Details(string userId)
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
            var user = _context.Users.Include(u=>u.Department).Where(u=>u.Id == userId ).FirstOrDefault();
            if (user == null)
            {
                return NotFound(); // or handle the case when the user is not found
            }

            // Get the user roles
            var roles = await _userManager.GetRolesAsync(user);

            var model = new UserRolesViewModel
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Department = user.Department,
                Roles = roles.ToList()
            };
           
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string userId)
        {
            //First Fetch the User Details by UserId
            var user = await _userManager.FindByIdAsync(userId);
            //Check if User Exists in the Database
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
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

        [HttpPost]
        public async Task<IActionResult> ChangeEmail(string userId, string newEmail)
        {
            // 1. Ensure User is Authenticated
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login"); // Redirect to login if not authenticated
            }

            // 2. Update Email in UserManager
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(); // User not found
            }

            // 3. Handle Unique Email
            var existingUser = await _userManager.FindByEmailAsync(newEmail);
            if (existingUser != null && existingUser.Id != user.Id)
            {
                return RedirectToAction("Details", new { userId, error= "Email is already in use." });
            }

            // Update the email address
            user.Email = newEmail;
            user.NormalizedEmail = newEmail.ToUpperInvariant();
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                // Handle error, e.g., ModelState.AddModelError
                return RedirectToAction("Details", new { userId, error = "Email not change." });
            }

            // 4. Send Email Confirmation (Optional)
            // You can implement email confirmation logic here if needed
            return RedirectToAction("Details", new { userId, success="Email has been changed." });
        }

        [HttpPost]
        public IActionResult ChangeDepartment(string userId, string selectedValue)
        {
            // Validate input parameters
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(selectedValue))
            {
                // Handle invalid input, for example, return a BadRequest result
                return BadRequest("Invalid input parameters");
            }

            // Retrieve the user by userId
            var user = _userManager.FindByIdAsync(userId).Result;
            if (user == null)
            {
                // Handle user not found, for example, return a NotFound result
                return NotFound($"User with ID {userId} not found");
            }


            // Retrieve the department by departmentId
            var department = _context.Departments.Find(selectedValue);
            if (department == null)
            {
                // Handle department not found, for example, return a NotFound result
                return RedirectToAction("Details", new { userId, error = $"Department with ID {selectedValue} not found" });
            }

            // Update the user's department
            user.Department = department;

            // Save changes to the database
            var result = _userManager.UpdateAsync(user).Result;

            if (result.Succeeded)
            {
                // Handle successful update, for example, return a success message
                return RedirectToAction("Details", new { userId, success = "Department updated successfully." });
            }
            else
            {
                // Handle update failure, for example, return an error message
                return RedirectToAction("Details", new { userId, error = "Failed to update department" });
            }
        }

        private string GenerateConfirmationEmail(string callbackUrl)
        {
            return $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Email Confirmation</title>
    <style>
        body{{font-family:Arial, sans-serif;background-color:#f4f4f4;color:#333;margin:0;padding:0;}}
        .container{{max-width:600px;margin:0 auto;padding:20px;}}
        .header{{background-color:#4285f4;color:#fff;padding:10px;text-align:center;}}
        .content{{background-color:#fff;padding:20px;border-radius:5px;box-shadow:0 0 10px rgba(0, 0, 0, 0.1);}}
        .footer{{text-align:center;margin-top:20px;color:#888;}}
        a{{color:#4285f4;text-decoration:none;}}
        a:hover{{text-decoration:underline;}}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'><h1>Email Confirmation</h1></div>
        <div class='content'>
            <p>Hello,</p>
            <p>Thank you for registering with our platform. Please click the link below to confirm your email address:</p>
            <p><a href='{callbackUrl}'>Confirm Email</a></p>
            <p>If you did not register, you can safely ignore this email. Your account will not be activated.</p>
        </div>
        <div class='footer'><p>&copy; Training App</p></div>
    </div>
</body>
</html>
";
        }

        private string GenerateSetPasswordEmail(string callbackUrl)
        {
            return $@"
        <!DOCTYPE html>
        <html lang='en'>
            <head>
                <meta charset='UTF-8'>
                <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                <title>Set Password</title>
                <style>
                    body{{font-family:Arial, sans-serif;background-color:#f4f4f4;color:#333;margin:0;padding:0;}}
                    .container{{max-width:600px;margin:0 auto;padding:20px;}}
                    .header{{background-color:#4285f4;color:#fff;padding:10px;text-align:center;}}
                    .content{{background-color:#fff;padding:20px;border-radius:5px;box-shadow:0 0 10px rgba(0, 0, 0, 0.1);}}
                    .footer{{text-align:center;margin-top:20px;color:#888;}}
                    a{{color:#4285f4;text-decoration:none;}}
                    a:hover{{text-decoration:underline;}}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'><h1>Set Password</h1></div>
                    <div class='content'>
                        <p>Hello,</p>
                        <p>You have been invited to set your password. Please click the link below to set your password:</p>
                        <p><a href='{callbackUrl}'>Set Password</a></p>
                        <p>If you did not request this, you can safely ignore this email. Your password will not be changed.</p>
                    </div>
                    <div class='footer'><p>&copy; Training App</p></div>
                </div>
            </body>
        </html>
    ";
        }
    }
}