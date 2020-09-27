using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PM.MVC.Models.EF
{
    public class PMAppDbContext : DbContext
    {
        public PMAppDbContext(DbContextOptions<PMAppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Project> Projects { get; set; }

        public DbSet<Resource> Resources { get; set; }

        public DbSet<Skill> Skills { get; set; }

        public DbSet<Qualification> Qualifications { get; set; }

        public DbSet<ProjectQualification> ProjectQualifications { get; set; }

        public DbSet<ProjectResource> ProjectResources { get; set; }

        public DbSet<QualificationResource> QualificationResources { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProjectQualification>().HasKey(x => new { x.ProjectId, x.QualificationId });
            modelBuilder.Entity<ProjectQualification>().HasOne(x => x.Project).WithMany(x => x.ProjectQualifications).HasForeignKey(x => x.ProjectId);
            modelBuilder.Entity<ProjectQualification>().HasOne(x => x.Qualification).WithMany(x => x.ProjectQualification).HasForeignKey(x => x.QualificationId);

            modelBuilder.Entity<ProjectResource>().HasKey(x => new { x.ProjectId, x.ResourceId });
            modelBuilder.Entity<ProjectResource>().HasOne(x => x.Project).WithMany(x => x.ProjectResources).HasForeignKey(x => x.ProjectId);
            modelBuilder.Entity<ProjectResource>().HasOne(x => x.Resource).WithMany(x => x.ProjectResources).HasForeignKey(x => x.ResourceId);

            modelBuilder.Entity<QualificationResource>().HasKey(x => new { x.QualificationId, x.ResourceId });
            modelBuilder.Entity<QualificationResource>().HasOne(x => x.Qualification).WithMany(x => x.QualificationResources).HasForeignKey(x => x.QualificationId);
            modelBuilder.Entity<QualificationResource>().HasOne(x => x.Resource).WithMany(x => x.QualificationResources).HasForeignKey(x => x.ResourceId);

            // initializing skills
            var csharpSkill = new Skill { Id = 1, Name = "C#" };
            var excelSkill = new Skill { Id = 2, Name = "Excel" };
            var drivingSkill = new Skill { Id = 3, Name = "Driving licence" };
            var managerSkill = new Skill { Id = 4, Name = "Project management" };

            modelBuilder.Entity<Skill>().HasData(new List<Skill> { csharpSkill, excelSkill, drivingSkill, managerSkill });

            // initializing qualifications
            var csharpJunior = new Qualification { Id = 1, Level = SkillLevel.Junior, SkillId = csharpSkill.Id};
            var excelJunior = new Qualification { Id = 2, Level = SkillLevel.Junior, SkillId = excelSkill.Id };
            var drivingJunior = new Qualification { Id = 3, Level = SkillLevel.Junior, SkillId = drivingSkill.Id };
            var managerJunior = new Qualification { Id = 4, Level = SkillLevel.Junior, SkillId = managerSkill.Id };

            var csharpMiddle = new Qualification { Id = 5, Level = SkillLevel.Middle, SkillId = csharpSkill.Id };
            var excelMiddle = new Qualification { Id = 6, Level = SkillLevel.Middle, SkillId = excelSkill.Id };
            var drivingMiddle = new Qualification { Id = 7, Level = SkillLevel.Middle, SkillId = drivingSkill.Id };
            var managerMiddle = new Qualification { Id = 8, Level = SkillLevel.Middle, SkillId = managerSkill.Id };

            var csharpSenior = new Qualification { Id = 9, Level = SkillLevel.Senior, SkillId = csharpSkill.Id };
            var excelSenior = new Qualification { Id = 10, Level = SkillLevel.Senior, SkillId = excelSkill.Id };
            var drivingSenior = new Qualification { Id = 11, Level = SkillLevel.Senior, SkillId = drivingSkill.Id };
            var managerSenior = new Qualification { Id = 12, Level = SkillLevel.Senior, SkillId = managerSkill.Id };

            modelBuilder.Entity<Qualification>()
            .HasData(new List<Qualification>
            {
                csharpJunior,
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
                managerSenior,
            });

            // initializing resources
            var steveResource = new Resource { Id = 1, Name = "Steve" };
            var bobResource = new Resource { Id = 2, Name = "Bob" };
            var jhonResource = new Resource { Id = 3, Name = "Jhon" };
            var katrinResource = new Resource { Id = 4, Name = "Katrin" };
            modelBuilder.Entity<Resource>().HasData(new List<Resource> { steveResource, katrinResource, bobResource, jhonResource });

            // initializing project
            Project spaceProject = new Project { Id = 1, Name = "Space Project", };
            var projectQualifications = InitProjectQualification(spaceProject, csharpJunior, drivingMiddle, managerJunior, excelMiddle);
            modelBuilder.Entity<Project>().HasData(spaceProject);
            modelBuilder.Entity<ProjectQualification>().HasData(projectQualifications);

            var projectResources = InitProjectResource(spaceProject, steveResource, katrinResource);
            modelBuilder.Entity<ProjectResource>().HasData(projectResources);

            var steveQualificationResources = InitQualificationResources(steveResource, csharpJunior, drivingMiddle);
            var katrinQualificationResources = InitQualificationResources(katrinResource, drivingMiddle);
            var bobQualificationResources = InitQualificationResources(bobResource, excelMiddle);
            var jhonQualificationResources = InitQualificationResources(jhonResource, managerJunior, excelMiddle);
            modelBuilder.Entity<QualificationResource>().HasData(steveQualificationResources);
            modelBuilder.Entity<QualificationResource>().HasData(katrinQualificationResources);
            modelBuilder.Entity<QualificationResource>().HasData(bobQualificationResources);
            modelBuilder.Entity<QualificationResource>().HasData(jhonQualificationResources);
        }

        private static IEnumerable<ProjectQualification> InitProjectQualification(Project project, params Qualification[] qualifications)
        {
            var projectQualifications = new List<ProjectQualification>();
            foreach (Qualification qualification in qualifications)
            {
                projectQualifications.Add(new ProjectQualification
                {
                    QualificationId = qualification.Id, ProjectId = project.Id
                });
            }

            return projectQualifications;
        }

        private static List<ProjectResource> InitProjectResource(Project project, params Resource[] resources)
        {
            var projectResources = new List<ProjectResource>();
            foreach (Resource resource in resources)
            {
                projectResources.Add(new ProjectResource { ResourceId = resource.Id, ProjectId = project.Id });
            }

            return projectResources;
        }

        private static List<QualificationResource> InitQualificationResources(Resource resource, params Qualification[] qualifications)
        {
            var qualificationResources = new List<QualificationResource>();
            foreach (Qualification qualification in qualifications)
            {
                qualificationResources.Add(new QualificationResource
                {
                    QualificationId = qualification.Id,ResourceId = resource.Id
                });
            }

            return qualificationResources;
        }
    }
}