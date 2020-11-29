using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using PM.MVC.Models.EF;
using PM.MVC.Models.Interfaces;
using PM.MVC.ViewModels;

namespace PM.MVC.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityResource> _signInManager;
        private readonly UserManager<IdentityResource> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IRepository<Qualification> _qualificationRepository;
        private readonly IQualificationService<IdentityResource> _qualificationService;

        public RegisterModel(UserManager<IdentityResource> userManager, SignInManager<IdentityResource> signInManager, ILogger<RegisterModel> logger, IEmailSender emailSender,
                                     IRepository<Qualification> qualificationRepository, IQualificationService<IdentityResource> qualificationService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _qualificationRepository = qualificationRepository;
            _qualificationService = qualificationService;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public MultiSelectViewModel Qualifications { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            public MultiSelectViewModel Qualifications { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            IEnumerable<Qualification> qualifications = await _qualificationRepository.GetAllAsync();
            Qualifications = new MultiSelectViewModel
            {
                Elements = qualifications.Select(x => new SelectListItem { Text = $"{x.Skill.Name} - {x.Level}", Value = x.Id.ToString() }).ToList(),
                Ids = new int[0]
            };
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = new IdentityResource { UserName = Input.Email, Email = Input.Email };
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (Input.Qualifications != null)
                {
                    int[] choosedQualificationsIds = Input.Qualifications.Ids;
                    await _qualificationService.AddQualificationsAsync(user, choosedQualificationsIds);
                }

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page("/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email,
                        "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            IEnumerable<Qualification> qualifications = await _qualificationRepository.GetAllAsync();
            Qualifications = new MultiSelectViewModel
            {
                Elements = qualifications.Select(x => new SelectListItem { Text = $"{x.Skill.Name} - {x.Level}", Value = x.Id.ToString() }).ToList(),
                Ids = new int[0]
            };

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
