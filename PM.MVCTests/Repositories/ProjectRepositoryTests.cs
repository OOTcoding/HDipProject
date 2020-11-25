using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using PM.MVC.Models.EF;
using PM.MVC.Models.Repositories;

namespace PM.MVCTests.Repositories
{
    [TestFixture]
    public class ProjectRepositoryTests : RepositoryTest
    {
        public ProjectRepositoryTests()
            : base(new DbContextOptionsBuilder<PMAppDbContext>().UseInMemoryDatabase(databaseName: "ProjectManageDataBase").Options)
        {
        }

        [Test]
        public async Task GetOneAsyncTest()
        {
            await using var context = new PMAppDbContext(ContextOptions);
            ProjectRepository repository = new ProjectRepository(context);

            var project = await repository.GetOneAsync(1);

            Assert.AreEqual("Space Project", project.Name);
        }

        [Test]
        public async Task GetAllAsyncTest()
        {
            await using var context = new PMAppDbContext(ContextOptions);
            ProjectRepository repository = new ProjectRepository(context);

            IEnumerable<Project> projects = await repository.GetAllAsync();

            Assert.AreEqual(1, projects.Count());
        }

        [Test]
        public async Task AddAsyncTest()
        {
            await using var context = new PMAppDbContext(ContextOptions);
            ProjectRepository repository = new ProjectRepository(context);

            var request = new Project { Name = "New project", FromDuration = new DateTime(2020, 5, 18), ToDuration = new DateTime(2021, 1, 15) };
            var result = await repository.AddAsync(request);
            Assert.AreEqual(request.Name, result.Name);

            IEnumerable<Project> projects = await repository.GetAllAsync();
            result = projects.FirstOrDefault(x => x.Name == request.Name);
            Assert.NotNull(result);
            Assert.AreEqual(request.Name, result.Name);
        }

        [Test]
        public async Task UpdateAsyncTest()
        {
            await using var context = new PMAppDbContext(ContextOptions);
            ProjectRepository repository = new ProjectRepository(context);

            var updateRequest = new Project { Name = "ChangedName", Id = 1 };

            var result = await repository.UpdateAsync(updateRequest);
            Assert.AreEqual(updateRequest.Name, result.Name);

            IEnumerable<Project> projects = await repository.GetAllAsync();
            result = projects.FirstOrDefault(x => x.Name == updateRequest.Name);
            Assert.NotNull(result);
            Assert.AreEqual(updateRequest.Name, result.Name);
        }

        [Test]
        public async Task DeleteAsyncTest()
        {
            await using var context = new PMAppDbContext(ContextOptions);
            ProjectRepository repository = new ProjectRepository(context);

            await repository.DeleteAsync(1);

            IEnumerable<Project> projects = await repository.GetAllAsync();

            Assert.IsTrue(!projects.Any());
        }
    }
}