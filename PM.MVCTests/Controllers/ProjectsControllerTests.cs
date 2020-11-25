using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using PM.MVC.Controllers;
using PM.MVC.Models.EF;
using PM.MVC.Models.Exceptions;
using PM.MVC.Models.Interfaces;
using PM.MVC.ViewModels;

namespace PM.MVCTests.Controllers
{
    [TestFixture]
    public class ProjectsControllerTests
    {
        private readonly ProjectsController _controller = GetProjectsController();

        [Test]
        public async Task Index_Action_Returns_View_With_ProjectsIEnumerable()
        {
            var actionResult = await _controller.Index() as ViewResult;
            Assert.NotNull(actionResult);
            Assert.IsInstanceOf<IEnumerable<Project>>(actionResult.Model);
        }

        [Test]
        public void Details_Should_Return_NotFound()
        {
            Assert.ThrowsAsync<RequestedResourceNotFoundException>(async () => await _controller.Details(21));
        }

        [Test]
        public async Task Details_Should_Return_Correct_Project()
        {
            var actionResult = await _controller.Details(1) as ViewResult;
            var product = (Project)actionResult?.ViewData.Model;
            Assert.AreEqual("Space Project", product?.Name);
        }

        [Test]
        public async Task Create_New_Project_RedirectsToIndex()
        {
            var project = new EditProjectViewModel { Id = 2, Name = "Office 2", FromDuration = new DateTime(2020, 2, 10), ToDuration = new DateTime(2021, 5, 15) };
            var result = await _controller.Create(project) as RedirectToActionResult;

            Assert.NotNull(result);
            Assert.AreEqual("Index", result.ActionName);
        }

        [Test]
        public async Task EditTest_RedirectsToIndex()
        {
            var actionResult = await _controller.Edit(1) as ViewResult;

            if (actionResult?.Model is EditProjectViewModel project)
            {
                project.Name = "ChangedName";

                var result = await _controller.Edit(project) as RedirectToActionResult;

                Assert.NotNull(result);
                Assert.AreEqual("Index", result.ActionName);
            }
        }

        private static ProjectsController GetProjectsController()
        {
            var mockProjectRepository = new Mock<IRepository<Project>>();
            mockProjectRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(MockProjectControllerData.GetFakeProjectData());
            mockProjectRepository.Setup(x => x.GetOneAsync(1)).ReturnsAsync(MockProjectControllerData.GetFakeProjectData().First());
            mockProjectRepository.Setup(x => x.GetOneAsync(21)).Throws<RequestedResourceNotFoundException>();

            var project = new Project { Id = 2, Name = "Office 2", FromDuration = new DateTime(2020, 2, 10), ToDuration = new DateTime(2021, 5, 15) };

            mockProjectRepository.Setup(x => x.AddAsync(project)).ReturnsAsync(project);
            mockProjectRepository.Setup(x => x.UpdateAsync(project)).ReturnsAsync(project);


            var mockResourceService = new Mock<IResourceService<Project>>();
            var mockQualificationService = new Mock<IQualificationService<Project>>();

            var userStoreMock = Mock.Of<IUserStore<IdentityResource>>();
            var userMgr = new Mock<UserManager<IdentityResource>>(userStoreMock,
                                                                  null,
                                                                  null,
                                                                  null,
                                                                  null,
                                                                  null,
                                                                  null,
                                                                  null,
                                                                  null);
            var user = new IdentityResource { Id = "f00", UserName = "Steve", Email = "steve@gmail.com" };
            var tcs = new TaskCompletionSource<IdentityResource>();
            tcs.SetResult(user);
            userMgr.Setup(x => x.FindByIdAsync("f00")).Returns(tcs.Task);

            var tcsRole = new TaskCompletionSource<bool>();
            tcsRole.SetResult(true);
            userMgr.Setup(x => x.IsInRoleAsync(user, "Manager")).Returns(tcsRole.Task);

            Mock<ClaimsPrincipal> claimsPrincipalMock = new Mock<ClaimsPrincipal>();
            claimsPrincipalMock.Setup(x => x.IsInRole("Manager")).Returns(true);
            claimsPrincipalMock.Name = "Manager";
            claimsPrincipalMock.SetupGet(x => x.Identity.Name).Returns(claimsPrincipalMock.Name);

            userMgr.Setup(x => x.GetUserAsync(claimsPrincipalMock.Object)).Returns(tcs.Task);

            var context = new ControllerContext { HttpContext = new DefaultHttpContext { User = claimsPrincipalMock.Object } };

            var mockNotificationRepository = new Mock<INotificationRepository>();
            var mockQualificationExcelService = new Mock<IExcelService<Qualification>>();
            var mockIdentityResourceExcelService = new Mock<IExcelService<IdentityResource>>();

            var projectsController = new ProjectsController(mockProjectRepository.Object,
                                                            mockResourceService.Object,
                                                            mockQualificationService.Object,
                                                            userMgr.Object,
                                                            mockNotificationRepository.Object,
                                                            mockQualificationExcelService.Object,
                                                            mockIdentityResourceExcelService.Object)
            { ControllerContext = context };

            return projectsController;
        }
    }
}