using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PM.MVC.Models;
using PM.MVC.Models.EF;
using PM.MVC.Models.Interfaces;
using PM.MVC.ViewModels;

namespace PM.MVC.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        private readonly IRepository<Project> _projectRepository;
        private readonly IResourceService<Project> _resourceService;
        private readonly IQualificationService<Project> _qualificationService;
        private readonly UserManager<IdentityResource> _userManager;
        private readonly INotificationRepository _notificationRepository;
        private readonly IExcelService<Qualification> _qualificationExcelService;
        private readonly IExcelService<IdentityResource> _identityResourceExcelService;

        public ProjectsController(IRepository<Project> projectRepository, IResourceService<Project> resourceService, IQualificationService<Project> qualificationService,
            UserManager<IdentityResource> userManager, INotificationRepository notificationRepository, IExcelService<Qualification> qualificationExcelService,
                                  IExcelService<IdentityResource> identityResourceExcelService)

        {
            _projectRepository = projectRepository;
            _resourceService = resourceService;
            _qualificationService = qualificationService;
            _userManager = userManager;
            _notificationRepository = notificationRepository;
            _qualificationExcelService = qualificationExcelService;
            _identityResourceExcelService = identityResourceExcelService;
        }

        // GET: Projects
        public async Task<IActionResult> Index()
        {
            IdentityResource identityResource = await _userManager.GetUserAsync(User);
            IEnumerable<Project> projects = await _projectRepository.GetAllAsync();

            bool isInManagerRole = await _userManager.IsInRoleAsync(identityResource, "Manager");
            if (!isInManagerRole)
            {
                IEnumerable<Project> resourceProjects = projects.Where(x => x.ProjectResources.SingleOrDefault(r => r.IdentityResourceId == identityResource.Id) != null);
                return View(resourceProjects);
            }

            return View(projects);
        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var project = await _projectRepository.GetOneAsync(id);
            return View(project);
        }

        public async Task<IActionResult> ExportToExcel(int id)
        {
            var project = await _projectRepository.GetOneAsync(id);

            DataTable resourcesDataTable = _identityResourceExcelService.GetTable(project.ProjectResources.Select(x => x.IdentityResource));
            DataTable qualificationsDataTable = _qualificationExcelService.GetTable(project.ProjectQualifications.Select(x => x.Qualification));

            byte[] content = ExcelHelper.GetContentAsBytes(resourcesDataTable, qualificationsDataTable);

            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "project-details.xlsx");
        }
        // GET: Projects/Create
        [Authorize(Roles = "Manager")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Projects/Create
        [Authorize(Roles = "Manager")]
        [HttpPost]
        public async Task<IActionResult> Create(EditProjectViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityResource user = await _userManager.GetUserAsync(User);

                Project addProject = new Project { Name = model.Name, FromDuration = model.FromDuration, ToDuration = model.ToDuration, ManagerId = user.Id, Manager = user };

                await _projectRepository.AddAsync(addProject);
                return RedirectToAction("Index");
            }

            return View(model);
        }

        // GET: Projects/Edit/5
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Edit(int id)
        {
            Project project = await _projectRepository.GetOneAsync(id);

            var model = new EditProjectViewModel
            {
                Id = project.Id, Name = project.Name, FromDuration = project.FromDuration, ToDuration = project.ToDuration, ManagerId = project.ManagerId
            };

            foreach (IdentityResource user in _userManager.Users)
            {
                if (await _userManager.IsInRoleAsync(user, "Manager") && user.Id != project.ManagerId)
                {
                    model.Users.Add(user);
                }
            }

            return View(model);
        }

        // POST: Projects/Edit/5
        [Authorize(Roles = "Manager")]
        [HttpPost]
        public async Task<IActionResult> Edit(EditProjectViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            Project updateProject = new Project { Id = model.Id, Name = model.Name, FromDuration = model.FromDuration, ToDuration = model.ToDuration, ManagerId = model.ManagerId };

            await _projectRepository.UpdateAsync(updateProject);
            return RedirectToAction("Index");
        }

        // POST: Projects/Delete/5
        [Authorize(Roles = "Manager")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _projectRepository.DeleteAsync(id);
            return RedirectToAction("Index");
        }

        [HttpGet("Projects/{id:int:min(1)}/resources")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> AddResources(int id)
        {
            var resources = await _resourceService.GetSuitableResourcesAsync(await _projectRepository.GetOneAsync(id));

            return View(new SuitableProjectResourceViewModel
            {
                SuitableProjectResourceList = resources.Select(x => new SuitableProjectResourceListItem { IdentityResource = x, IsChecked = false }).ToList()
            });
        }

        [HttpPost("Projects/{id:int:min(1)}/resources")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> AddResources(int id, SuitableProjectResourceViewModel resourcesToAdd)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var identityResourcesIds = resourcesToAdd.SuitableProjectResourceList.Where(x => x.IsChecked).Select(x => x.IdentityResource.Id).ToList();

            string user = _userManager.GetUserName(User);
            var notification = new Notification { Text = $"You were assigned to the project by {user}" };
            await _notificationRepository.Create(notification, identityResourcesIds);

            await _resourceService.AddResourcesAsync(await _projectRepository.GetOneAsync(id), identityResourcesIds);
            return RedirectToAction("Details", new { id });
        }

        [HttpGet("Projects/{id:int:min(1)}/qualifications")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> AddQualifications(int id)
        {
            var qualifications = await _qualificationService.GetSuitableQualificationsAsync(await _projectRepository.GetOneAsync(id));

            return View(new ChooseQualificationsViewModel
            {
                SuitableQualifications = qualifications.Select(x => new ChooseQualificationsListItem { Qualification = x, IsChecked = false }).ToList()
            });
        }

        [HttpPost("Projects/{id:int:min(1)}/qualifications")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> AddQualifications(int id, ChooseQualificationsViewModel qualificationsToAdd)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            await _qualificationService.AddQualificationsAsync(await _projectRepository.GetOneAsync(id), qualificationsToAdd.SuitableQualifications.Where(x => x.IsChecked).Select(x => x.Qualification.Id));
            return RedirectToAction("Details", new { id });
        }

        [HttpPost("Projects/{id:int:min(1)}/qualifications/{qualificationId:int:min(1)}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> DeleteQualification(int id, int qualificationId)
        {
            await _qualificationService.DeleteQualificationAsync(await _projectRepository.GetOneAsync(id), qualificationId); 
            return RedirectToAction("Details", new { id });
        }

        [HttpPost("Projects/{id:int:min(1)}/resources/{resourceId}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> DeleteResource(int id, string resourceId)
        {
            await _resourceService.DeleteResourceAsync(await _projectRepository.GetOneAsync(id), resourceId);

            string user = _userManager.GetUserName(User);
            var notification = new Notification { Text = $"You were deleted from the project by {user}" };

            await _notificationRepository.Create(notification, new List<string> { resourceId });

            return RedirectToAction("Details", new { id });
        }
    }
}