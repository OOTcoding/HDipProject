using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using PM.MVC.Models.EF;

namespace PM.MVCTests.Repositories
{
    public class RepositoryTest
    {
        protected RepositoryTest(DbContextOptions<PMAppDbContext> contextOptions)
        {
            ContextOptions = contextOptions;
        }

        [SetUp]
        public void BaseSetUp()
        {
            Seed().Wait();
        }

        protected DbContextOptions<PMAppDbContext> ContextOptions { get; }

        private async Task Seed()
        {
            await using var context = new PMAppDbContext(ContextOptions);
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            // initializing resources
            var steveResource = new IdentityResource { UserName = "Steve", Email = "steve@gmail.com" };
            var bobResource = new IdentityResource { UserName = "Bob", Email = "bob@gmail.com" };
            var jhonResource = new IdentityResource { UserName = "Jhon", Email = "jhon@gmail.com" };
            var katrinResource = new IdentityResource { UserName = "Katrin", Email = "katrin@gmail.com" };

            await context.AddRangeAsync(steveResource, bobResource, jhonResource, katrinResource);

            // initializing skills
            var csharpSkill = new Skill { Name = "C#" };
            var excelSkill = new Skill { Name = "Excel" };
            var drivingSkill = new Skill { Name = "Driving licence" };
            var managerSkill = new Skill { Name = "Project management" };

            await context.Skills.AddRangeAsync(csharpSkill, excelSkill, drivingSkill, managerSkill);

            // initializing qualifications
            var csharpJunior = new Qualification { Level = SkillLevel.Junior, Skill = csharpSkill };
            var excelJunior = new Qualification { Level = SkillLevel.Junior, Skill = excelSkill };
            var drivingJunior = new Qualification { Level = SkillLevel.Junior, Skill = drivingSkill };
            var managerJunior = new Qualification { Level = SkillLevel.Junior, Skill = managerSkill };

            var csharpMiddle = new Qualification { Level = SkillLevel.Middle, Skill = csharpSkill };
            var excelMiddle = new Qualification { Level = SkillLevel.Middle, Skill = excelSkill };
            var drivingMiddle = new Qualification { Level = SkillLevel.Middle, Skill = drivingSkill };
            var managerMiddle = new Qualification { Level = SkillLevel.Middle, Skill = managerSkill };

            var csharpSenior = new Qualification { Level = SkillLevel.Senior, Skill = csharpSkill };
            var excelSenior = new Qualification { Level = SkillLevel.Senior, Skill = excelSkill };
            var drivingSenior = new Qualification { Level = SkillLevel.Senior, Skill = drivingSkill };
            var managerSenior = new Qualification { Level = SkillLevel.Senior, Skill = managerSkill };

            await context.Qualifications.AddRangeAsync(csharpJunior,
                                                       excelJunior,
                                                       drivingJunior,
                                                       managerJunior,
                                                       csharpMiddle,
                                                       excelMiddle,
                                                       drivingMiddle,
                                                       managerMiddle,
                                                       csharpSenior,
                                                       excelSenior,
                                                       drivingSenior,
                                                       managerSenior);

            // initializing project
            Project spaceProject = new Project
            {
                Name = "Space Project", Manager = steveResource, FromDuration = new DateTime(2020, 2, 10), ToDuration = new DateTime(2020, 8, 5)
            };
            var projectQualifications = InitProjectQualification(spaceProject, csharpJunior, drivingMiddle, managerJunior, excelMiddle);
            await context.Projects.AddAsync(spaceProject);
            await context.ProjectQualifications.AddRangeAsync(projectQualifications);

            var projectResources = InitProjectResource(spaceProject, steveResource, katrinResource);
            await context.ProjectIdentityResources.AddRangeAsync(projectResources);

            var steveQualificationResources = InitQualificationResources(steveResource, csharpJunior, drivingMiddle);
            var katrinQualificationResources = InitQualificationResources(katrinResource, drivingMiddle);
            var bobQualificationResources = InitQualificationResources(bobResource, excelMiddle);
            var jhonQualificationResources = InitQualificationResources(jhonResource, managerJunior, excelMiddle);
            await context.QualificationIdentityResources.AddRangeAsync(steveQualificationResources);
            await context.QualificationIdentityResources.AddRangeAsync(katrinQualificationResources);
            await context.QualificationIdentityResources.AddRangeAsync(bobQualificationResources);
            await context.QualificationIdentityResources.AddRangeAsync(jhonQualificationResources);

            await context.SaveChangesAsync();
        }

        private static IEnumerable<ProjectQualification> InitProjectQualification(Project project, params Qualification[] qualifications)
        {
            return qualifications.Select(qualification => new ProjectQualification { Qualification = qualification, Project = project }).ToList();
        }

        private static IEnumerable<ProjectIdentityResource> InitProjectResource(Project project, params IdentityResource[] resources)
        {
            return resources.Select(resource => new ProjectIdentityResource { IdentityResource = resource, Project = project }).ToList();
        }

        private static IEnumerable<QualificationIdentityResource> InitQualificationResources(IdentityResource identityResource, params Qualification[] qualifications)
        {
            return qualifications.Select(qualification => new QualificationIdentityResource { Qualification = qualification, IdentityResource = identityResource }).ToList();
        }
    }
}