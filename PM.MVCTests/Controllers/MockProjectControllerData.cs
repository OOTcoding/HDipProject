using System;
using System.Collections.Generic;
using System.Linq;
using PM.MVC.Models.EF;

namespace PM.MVCTests.Controllers
{
    public class MockProjectControllerData
    {
        public static IEnumerable<Project> GetFakeProjectData()
        {
            var steveResource = new IdentityResource { UserName = "Steve", Email = "steve@gmail.com", Id = "1" };
            var bobResource = new IdentityResource { UserName = "Bob", Email = "bob@gmail.com", Id = "2" };
            var jhonResource = new IdentityResource { UserName = "Jhon", Email = "jhon@gmail.com", Id = "3" };
            var katrinResource = new IdentityResource { UserName = "Katrin", Email = "katrin@gmail.com", Id = "4" };
            var csharpSkill = new Skill { Name = "C#", Id = 1 };
            var excelSkill = new Skill { Name = "Excel", Id = 2 };
            var drivingSkill = new Skill { Name = "Driving licence", Id = 3 };
            var managerSkill = new Skill { Name = "Project management", Id = 4 };

            var csharpJunior = new Qualification { Level = SkillLevel.Junior, Skill = csharpSkill, Id = 1, SkillId = csharpSkill.Id };
            var managerJunior = new Qualification { Level = SkillLevel.Junior, Skill = managerSkill, Id = 2, SkillId = managerSkill.Id };
            var excelMiddle = new Qualification { Level = SkillLevel.Middle, Skill = excelSkill, Id = 3, SkillId = excelSkill.Id };
            var drivingMiddle = new Qualification { Level = SkillLevel.Middle, Skill = drivingSkill, Id = 4, SkillId = drivingSkill.Id };

            Project spaceProject = new Project
            {
                Id = 1, Name = "Space Project", Manager = steveResource, FromDuration = new DateTime(2020, 2, 10), ToDuration = new DateTime(2020, 8, 5)
            };
            var projectQualifications = InitProjectQualification(spaceProject, csharpJunior, drivingMiddle, managerJunior, excelMiddle);
            spaceProject.ProjectQualifications = projectQualifications;

            var projectResources = InitProjectResource(spaceProject, steveResource, katrinResource);
            spaceProject.ProjectResources = projectResources;

            var steveQualificationResources = InitQualificationResources(steveResource, csharpJunior, drivingMiddle);
            steveResource.QualificationResources = steveQualificationResources;

            var katrinQualificationResources = InitQualificationResources(katrinResource, drivingMiddle);
            katrinResource.QualificationResources = katrinQualificationResources;

            var bobQualificationResources = InitQualificationResources(bobResource, excelMiddle);
            bobResource.QualificationResources = bobQualificationResources;

            var jhonQualificationResources = InitQualificationResources(jhonResource, managerJunior, excelMiddle);
            jhonResource.QualificationResources = jhonQualificationResources;

            return new List<Project> { spaceProject };
        }

        private static List<ProjectQualification> InitProjectQualification(Project project, params Qualification[] qualifications)
        {
            return qualifications
                   .Select(qualification => new ProjectQualification
                   {
                       Qualification = qualification, Project = project, ProjectId = project.Id, QualificationId = qualification.Id
                   })
                   .ToList();
        }

        private static List<ProjectIdentityResource> InitProjectResource(Project project, params IdentityResource[] resources)
        {
            return resources.Select(resource => new ProjectIdentityResource
                            {
                                IdentityResource = resource, Project = project, ProjectId = project.Id, IdentityResourceId = resource.Id
                            })
                            .ToList();
        }

        private static List<QualificationIdentityResource> InitQualificationResources(IdentityResource identityResource, params Qualification[] qualifications)
        {
            return qualifications.Select(qualification => new QualificationIdentityResource
                                 {
                                     Qualification = qualification,
                                     IdentityResource = identityResource,
                                     IdentityResourceId = identityResource.Id,
                                     QualificationId = qualification.Id
                                 })
                                 .ToList();
        }
    }
}