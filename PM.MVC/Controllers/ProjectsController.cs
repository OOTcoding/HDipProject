using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PM.MVC.Models;
using PM.MVC.Models.EF;
using PM.MVC.Models.Services.Interfaces;
using PM.MVC.Models.ViewModels;

namespace PM.MVC.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly IProjectRepository _projectRepository;

        public ProjectsController(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
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
            var project = await _projectRepository.GetOneAsync(id);
            return View(project);
        }

        // POST: Projects/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(Project project)
        {
            if (!ModelState.IsValid)
            {
                return View(project);
            }

            await _projectRepository.UpdateAsync(project);
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}