#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using ReservationSystem.Models;

namespace ReservationSystem.Areas.Identity.Pages.Account
{
    // PageModel for the login page
    public class LoginModel : PageModel
    {
        // SignInManager for handling sign-in operations
        private readonly SignInManager<ApplicationUser> _signInManager;
        // UserManager for accessing user information
        private readonly UserManager<ApplicationUser> _userManager;
        // Logger for logging login events
        private readonly ILogger<LoginModel> _logger;

        // Constructor to inject dependencies
        public LoginModel(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ILogger<LoginModel> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        // Model binding for login form input
        [BindProperty]
        public InputModel Input { get; set; }

        // List of external authentication schemes (e.g., Google, Facebook)
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        // Return URL after successful login
        public string ReturnUrl { get; set; }

        // Error message to display on the page
        [TempData]
        public string ErrorMessage { get; set; }

        // Input model for login form fields
        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        // Handles GET requests to display the login page
        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            // Clear any existing external cookies
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            // Get available external login providers
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        // Handles POST requests to process login attempts
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            // Get available external login providers
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                // Attempt to sign in with the provided credentials
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");

                    // Get the user and redirect based on their role
                    var user = await _userManager.FindByEmailAsync(Input.Email);

                    if (await _userManager.IsInRoleAsync(user, "Admin"))
                        return LocalRedirect("/Admin/Index");
                    else if (await _userManager.IsInRoleAsync(user, "Instructor"))
                        return LocalRedirect("/Instructor/Index");
                    else
                        return LocalRedirect("/");
                }

                // Handle two-factor authentication requirement
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }

                // Handle locked out user
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }

                // Add error if login attempt failed
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }

            // If we got this far, something failed; redisplay form
            return Page();
        }
    }
}
