using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PM.MVC.Models;
using PM.MVC.Models.EF;
using PM.MVC.Models.Services.Interfaces;
using PM.MVC.ViewModels;

namespace PM.MVC.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        private readonly IProjectRepository _projectRepository;
        private readonly UserManager<IdentityUser> _userManager;

        public ProjectsController(IProjectRepository projectRepository, UserManager<IdentityUser> userManager)
        {
            _projectRepository = projectRepository;
            _userManager = userManager;
        }

        // GET: Projects
        public async Task<IActionResult> Index()
        {
            return View(await _projectRepository.GetAllAsync());
        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var project = await _projectRepository.GetOneAsync(id);
            return View(project);
        }

        // GET: Projects/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Projects/Create
        [HttpPost]
        public async Task<IActionResult> Create(Project project)
        {
            if (ModelState.IsValid)
            {
                await _projectRepository.AddAsync(project);
                return RedirectToAction("Index");
            }

            return View(project);
        }

        // GET: Projects/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            Project project = await _projectRepository.GetOneAsync(id);

            var model = new EditProjectViewModel
            {
                Id = project.Id, Name = project.Name, FromDuration = project.FromDuration, ToDuration = project.ToDuration, ManagerId = project.ManagerId
            };

            foreach (IdentityUser user in _userManager.Users)
            {
                if (await _userManager.IsInRoleAsync(user, "Administrators") && user.Id != project.ManagerId)
                {
                    model.Users.Add(user);
                }
            }

            return View(model);
        }

        // POST: Projects/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(EditProjectViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            Project updateProject = new Project
            {
                Id = model.Id, Name = model.Name, FromDuration = model.FromDuration, ToDuration = model.ToDuration, ManagerId = model.ManagerId
            };

            await _projectRepository.UpdateAsync(updateProject);
            return RedirectToAction("Index");
        }

        // POST: Projects/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _projectRepository.DeleteAsync(id);
            return RedirectToAction("Index");
        }

        [HttpGet("Projects/{id:int:min(1)}/resources")]
        public async Task<IActionResult> AddResources(int id)
        {
            var resources = await _projectRepository.GetSuitableResourcesAsync(id);

            return View(new SuitableProjectResourceViewModel
            {
                SuitableProjectResourceList = resources.Select(x => new SuitableProjectResourceListItem { Resource = x, IsChecked = false }).ToList()
            });
        }

        [HttpPost("Projects/{id:int:min(1)}/resources")]
        public async Task<ActionResult> AddResources(int id, SuitableProjectResourceViewModel resourcesToAdd)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            await _projectRepository.AddResourcesAsync(id, resourcesToAdd.SuitableProjectResourceList.Where(x => x.IsChecked).Select(x => x.Resource));
            return RedirectToAction("Details", new { id });
        }

        [HttpGet("Projects/{id:int:min(1)}/qualifications")]
        public async Task<IActionResult> AddQualifications(int id)
        {
            var qualifications = await _projectRepository.GetSuitableQualificationsAsync(id);

            return View(new ChooseQualificationsViewModel
            {
                SuitableQualifications = qualifications.Select(x => new ChooseQualificationsListItem { Qualification = x, IsChecked = false }).ToList()
            });
        }

        [HttpPost("Projects/{id:int:min(1)}/qualifications")]
        public async Task<IActionResult> AddQualifications(int id, ChooseQualificationsViewModel qualificationsToAdd)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            await _projectRepository.AddQualificationsAsync(id, qualificationsToAdd.SuitableQualifications.Where(x => x.IsChecked).Select(x => x.Qualification));
            return RedirectToAction("Details", new { id });
        }

        [HttpPost("Projects/{id:int:min(1)}/qualifications/{qualificationId:int:min(1)}")]
        public async Task<IActionResult> DeleteQualification(int id, int qualificationId)
        {
            await _projectRepository.DeleteQualificationAsync(id, qualificationId);
            return RedirectToAction("Details", new { id });
        }

        [HttpPost("Projects/{id:int:min(1)}/resources/{resourceId:int:min(1)}")]
        public async Task<IActionResult> DeleteResource(int id, int resourceId)
        {
            await _projectRepository.DeleteResourceAsync(id, resourceId);
            return RedirectToAction("Details", new { id });
        }
    }
}