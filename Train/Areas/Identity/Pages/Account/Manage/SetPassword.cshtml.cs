using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Train.Models.Identity;

namespace Train.Areas.Identity.Pages.Account.Manage
{
    [AllowAnonymous]
    public class SetPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public SetPasswordModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public class InputModel
        {
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "New password")]
            public string NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm new password")]
            [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }


        public async Task<IActionResult> OnGetAsync(string userId, string token)
        {
            // Validate the token and user ID
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || !await _userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultProvider, UserManager<ApplicationUser>.ResetPasswordTokenPurpose, token))
            {
                // Invalid token or user ID, redirect to login
                return RedirectToPage("./Login");
            }

            // Token is valid, set the EmailConfirmed property
            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);

            // Token is valid, proceed with setting the password
            var hasPassword = await _userManager.HasPasswordAsync(user);
            if (hasPassword)
            {
                // User already has a password, redirect to change password
                return RedirectToPage("./ChangePassword");
            }

            // User doesn't have a password, allow setting the password
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string userId, string token)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Validate the token and user ID again
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || !await _userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultProvider, UserManager<ApplicationUser>.ResetPasswordTokenPurpose, token))
            {
                // Invalid token or user ID, redirect to login
                return RedirectToPage("./Login");
            }

            var addPasswordResult = await _userManager.AddPasswordAsync(user, Input.NewPassword);
            if (!addPasswordResult.Succeeded)
            {
                foreach (var error in addPasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your password has been set.";

            // Redirect to the login page after setting the password
            return RedirectToPage("./Login");
        }

    }
}
