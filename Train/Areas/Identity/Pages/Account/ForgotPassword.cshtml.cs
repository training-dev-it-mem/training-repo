using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Train.Models;
using Train.Models.Identity;

namespace Train.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        public ForgotPasswordModel(UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToPage("./ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please 
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);

                var callbackUrl = Url.Page(
                    "/Account/ResetPassword",
                    pageHandler: null,
                    values: new { code },
                    protocol: Request.Scheme);

                await _emailSender.SendEmailAsync(
                    Input.Email,
                    "Reset Password",
                    $"<!DOCTYPE html><html lang=\"en\"><head><meta charset=\"UTF-8\"><meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\"><title>Password Reset</title><style>body{{font-family:Arial, sans-serif;background-color:#f4f4f4;color:#333;margin:0;padding:0;}}.container{{max-width:600px;margin:0 auto;padding:20px;}}.header{{background-color:#4285f4;color:#fff;padding:10px;text-align:center;}}.content{{background-color:#fff;padding:20px;border-radius:5px;box-shadow:0 0 10px rgba(0, 0, 0, 0.1);}}.footer{{text-align:center;margin-top:20px;color:#888;}}a{{color:#4285f4;text-decoration:none;}}a:hover{{text-decoration:underline;}}</style></head><body><div class=\"container\"><div class=\"header\"><h1>Password Reset</h1></div><div class=\"content\"><p>Hello,</p><p>You have requested to reset your password. Please click the link below to reset your password:</p><p><a href=\"{callbackUrl}\">Reset Password</a></p><p>If you did not request this, you can safely ignore this email. Your password will not be changed.</p></div><div class=\"footer\"><p>&copy; Training App</p></div></div></body></html>\r\n");

                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            return Page();
        }
    }
}
