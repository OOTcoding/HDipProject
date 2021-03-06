﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PM.MVC.Models.EF;

namespace PM.MVC.Models
{
    public static class DbInitializer
    {
        public static async Task Seed(IApplicationBuilder applicationBuilder)
        {
            using var serviceScope = applicationBuilder.ApplicationServices.CreateScope();
            var context = serviceScope.ServiceProvider.GetService<PMAppDbContext>();

            var userManager = serviceScope.ServiceProvider.GetService<UserManager<IdentityResource>>();
            var roleManager = serviceScope.ServiceProvider.GetService<RoleManager<IdentityRole>>();

            await context.Database.EnsureDeletedAsync();
            await context.Database.MigrateAsync();

            // initializing roles
            var managerRole = new IdentityRole("Manager");
            await roleManager.CreateAsync(managerRole);
            managerRole = await roleManager.FindByNameAsync(managerRole.Name);

            var resourceRole = new IdentityRole("Resource");
            await roleManager.CreateAsync(resourceRole);
            resourceRole = await roleManager.FindByNameAsync(resourceRole.Name);

            // initializing resources
            var steveResource = new IdentityResource { UserName = "Steve", Email = "steve@gmail.com" };
            var bobResource = new IdentityResource { UserName = "Bob", Email = "bob@gmail.com" };
            var jhonResource = new IdentityResource { UserName = "Jhon", Email = "jhon@gmail.com" };
            var katrinResource = new IdentityResource { UserName = "Katrin", Email = "katrin@gmail.com" };

            await userManager.CreateAsync(steveResource, "Pass123!");
            steveResource = await userManager.FindByEmailAsync(steveResource.Email);

            await userManager.CreateAsync(bobResource, "Pass123!");
            bobResource = await userManager.FindByEmailAsync(bobResource.Email);

            await userManager.CreateAsync(jhonResource, "Pass123!");
            jhonResource = await userManager.FindByEmailAsync(jhonResource.Email);

            await userManager.CreateAsync(katrinResource, "Pass123!");
            katrinResource = await userManager.FindByEmailAsync(katrinResource.Email);

            // adding to roles
            await userManager.AddToRoleAsync(steveResource, managerRole.Name);
            await userManager.AddToRoleAsync(bobResource, resourceRole.Name);
            await userManager.AddToRoleAsync(jhonResource, resourceRole.Name);
            await userManager.AddToRoleAsync(katrinResource, resourceRole.Name);

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
            Project spaceProject = new Project { Name = "Space Project", Manager = steveResource, FromDuration = new DateTime(2020, 2, 10), ToDuration = new DateTime(2020, 8, 5) };
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