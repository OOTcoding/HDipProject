using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PM.MVC.Models;
using PM.MVC.Models.EF;
using PM.MVC.Models.Services;
using PM.MVC.ViewModels;

namespace PM.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly SignInManager<IdentityResource> _signInManager;
        private readonly SummaryService _summaryService;
        public HomeController(SignInManager<IdentityResource> signInManager, SummaryService summaryService)
        {
            _signInManager = signInManager;
            _summaryService = summaryService;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            if (_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Summary");
            }

            return View();
        }

        [Authorize]
        public async Task<IActionResult> Summary()
        {
            return View(await _summaryService.GetProjectsPerManagerChartViewModelAsync());
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SummaryChartData(ChartDataViewModel viewModel)
        {
            if (viewModel.ButtonType == ButtonType.Project)
            {
                await _summaryService.GetProjectsPerManagerAsync(viewModel);
            }

            if (viewModel.ButtonType == ButtonType.Qualification)
            {
                await _summaryService.GetQualificationsByProjectAsync(viewModel);
            }

            if (viewModel.ButtonType == ButtonType.Resource)
            {
                await _summaryService.GetResourcesByProjectAsync(viewModel);
            }

            if (viewModel.ButtonType == ButtonType.Skill)
            {
                await _summaryService.GetSkillsByLevelAsync(viewModel);
            }

            return View("Summary", viewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}