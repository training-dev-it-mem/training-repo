using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Train.Data;
using Train.Models.Identity;
using Train.ViewModels;
using NuGet.Common;

namespace Train.Controllers
{
    public class RolesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        public RolesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Manage(string userId)
        {
            // Retrieve the user by their ID
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                // Handle the case where the user is not found
                return NotFound();
            }

            // Get the roles associated with the user
            var roles = await _userManager.GetRolesAsync(user);

            // You can now use 'roles' to display or process the user roles as needed
            // For example, you can pass 'roles' to the PartialView
            return PartialView("YourPartialViewName", roles);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRoles(ManageUserRolesViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user == null)
            {
                return NotFound();
            }

            var currentRoles = await _userManager.GetRolesAsync(user);

            // Remove user from current roles
            var resultRemove = await _userManager.RemoveFromRolesAsync(user, currentRoles);

            if (!resultRemove.Succeeded)
            {
                // Handle error
                var errorMessage = string.Join(", ", resultRemove.Errors.Select(e => e.Description));

                return RedirectToAction("Details", "Users", new { userId = model.UserId, error = errorMessage });
            }

            // Add user to selected roles
            var resultAdd = await _userManager.AddToRolesAsync(user, model.SelectedRoles);

            if (!resultAdd.Succeeded)
            {
                // Handle error
                var errorMessage = string.Join(", ", resultAdd.Errors.Select(e => e.Description));

                return RedirectToAction("Details", "Users", new { userId = model.UserId, error = errorMessage });
            }

            return RedirectToAction("Details", "Users", new { userId = model.UserId, success = "User roles has been updated." });

        }

        [HttpPost]
        public async Task<IActionResult> ChangeUserRoleUser(string userId, string role, bool isUserInRole)
        {
            // Your logic to add or remove the role for the user based on the values passed
            // Use userId, role, and isUserInRole parameters to update user roles

            // Example: Update user roles using UserManager
            var user = _userManager.FindByIdAsync(userId).Result;

            if (isUserInRole)
            {
                _userManager.RemoveFromRoleAsync(user, role).Wait();

                // Remove the password if the user role is removed
                _userManager.RemovePasswordAsync(user).Wait();
            }
            else
            {
                _userManager.AddToRoleAsync(user, role).Wait();

                // Generate email confirmation token
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);

                // Create a link with the token, and any other necessary information
                var callbackUrl = Url.Page("/Account/Manage/SetPassword", null, new { area = "Identity", userId = user.Id, code }, protocol: HttpContext.Request.Scheme);

                // Separate HTML message
                var htmlMessage = GenerateSetPasswordEmail(callbackUrl);

                // Send email to the user with the set password link
                await _emailSender.SendEmailAsync(user.Email, "Set Your Password", htmlMessage);

            }

            // Redirect to user details with success message
            return RedirectToAction("Details", "Users", new { userId = userId, success = $"{role} role has been added/removed" });
        }

        [HttpPost]
        public async Task<IActionResult> ChangeUserRoleManager(string userId, string role, bool isUserInRole)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                // Handle the case where the user is not found
                return NotFound();
            }

            if (isUserInRole)
            {
                // Remove the role and perform other actions if needed
                await _userManager.RemoveFromRoleAsync(user, role);
                // Remove the password if the user role is removed
                await _userManager.RemovePasswordAsync(user);
            }
            else
            {
                // Retrieve all users in the same department from the database
                var usersInDepartment = _context.Users.Where(u => u.DepartmentId == user.DepartmentId).ToList();

                // Check if any user in the same department already has the manager role
                var departmentHasManager = usersInDepartment.Any(u => _userManager.IsInRoleAsync(u, "Manager").Result);

                if (!departmentHasManager)
                {
                    // Add manager role only if the department doesn't have a user with the manager role
                    await _userManager.AddToRoleAsync(user, role);
                }
                else
                {
                    // Handle the case where the department already has a user with the manager role
                    return RedirectToAction("Details", "Users", new { userId = userId, error = "The department already has a user with the Manager role." });
                }
            }

            return RedirectToAction("Details", "Users", new { userId = userId, success = $"{role} role has been added/removed" });
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
