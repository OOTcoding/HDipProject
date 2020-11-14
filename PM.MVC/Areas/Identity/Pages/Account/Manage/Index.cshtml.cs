using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PM.MVC.Models.EF;
using PM.MVC.Models.Interfaces;
using PM.MVC.ViewModels;

namespace PM.MVC.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<IdentityResource> _userManager;
        private readonly SignInManager<IdentityResource> _signInManager;
        private readonly IQualificationService<IdentityResource> _qualificationService;
        private readonly PMAppDbContext _context;

        public IndexModel(UserManager<IdentityResource> userManager, SignInManager<IdentityResource> signInManager, IQualificationService<IdentityResource> qualificationService,
                          PMAppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _qualificationService = qualificationService;
            _context = context;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            public MultiSelectViewModel Qualifications { get; set; }
        }

        private async Task LoadAsync(IdentityResource user)
        {
            var userName = await _userManager.GetUserNameAsync(user);

            Qualification[] qualifications = await _context.Qualifications.Include(x => x.Skill).ToArrayAsync();

            QualificationIdentityResource[] qualificationIdentityResource = await _context.QualificationIdentityResources.Where(x => x.IdentityResourceId == user.Id).Include(x => x.Qualification).ToArrayAsync();

            List<SelectListItem> selectListItems = qualifications.Select(x => new SelectListItem
            {
                Text = $"{x.Skill.Name} - {x.Level}",
                Value = x.Id.ToString(),
                Disabled = qualificationIdentityResource.Any(q => q.Qualification.Skill.Id == x.SkillId && q.QualificationId != x.Id),
                Selected = qualificationIdentityResource.Any(q => q.QualificationId == x.Id)
            }).ToList();

            var choosedQualifications = new MultiSelectViewModel { Elements = selectListItems, Ids = selectListItems.Where(x => x.Selected).Select(x => int.Parse(x.Value)).ToArray() };

            Username = userName;

            Input = new InputModel { Qualifications = choosedQualifications };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            IdentityResource user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            QualificationIdentityResource[] qualificationIdentityResource = await
            _context.QualificationIdentityResources.Where(x => x.IdentityResourceId == user.Id).ToArrayAsync();
            _context.QualificationIdentityResources.RemoveRange(qualificationIdentityResource);
            await _context.SaveChangesAsync();

            int[] choosedQualificationsIds = Input.Qualifications.Ids;
            await _qualificationService.AddQualificationsAsync(user, choosedQualificationsIds);

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}