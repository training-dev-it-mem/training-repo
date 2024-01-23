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

                // Separate HTML message
                var htmlMessage = GenerateResetPasswordEmail(callbackUrl, user.Name);

                // Send email with the separated HTML message
                await _emailSender.SendEmailAsync(Input.Email, "Reset Password", htmlMessage);

                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            return Page();
        }
        private string GenerateResetPasswordEmail(string callbackUrl, string userName)
        {
            return $@"
        <!DOCTYPE html>
        <html lang='en'>
            <head>
                <meta charset='UTF-8'>
                <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                <title>Password Reset</title>
                <style>
                    body {{
                        font-family: Arial, sans-serif;
                        background-color: #f4f4f4;
                        color: #333;
                    }}
                    .container {{
                        max-width: 600px;
                    }}
                    .header {{
                        background-color: #5e72e4;
                        color: #fff;
                        text-align: center;
                    }}
                    .content {{
                        background-color: #fff;
                    }}
                    .reset-password-button {{
                        display: inline-block;
                        background-color: #5e72e4;
                        color: #fff;
                        text-decoration: none;
                        cursor: pointer;
                    }}
                    .footer {{
                        text-align: center;
                        color: #888;
                    }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'><h1>Password Reset</h1></div>
                    <div class='content'>
                        <p>Hello {userName},</p>
                        <p>You have requested to reset your password. Please click the link below to reset your password:</p>
                        <p><a href='{callbackUrl}' class='reset-password-button'>Reset Password</a></p>
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
