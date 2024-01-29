using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Train.Data;
using Train.Models.Identity;
using Train.ViewModels;

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
    }
}
