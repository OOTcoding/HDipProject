using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace PM.MVC.Models.EF
{
    public class PMAppDbContext : IdentityDbContext<IdentityResource>
    {
        public PMAppDbContext(DbContextOptions<PMAppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Project> Projects { get; set; }

        public DbSet<IdentityResource> IdentityResources { get; set; }

        public DbSet<Skill> Skills { get; set; }

        public DbSet<Qualification> Qualifications { get; set; }

        public DbSet<ProjectQualification> ProjectQualifications { get; set; }

        public DbSet<ProjectIdentityResource> ProjectIdentityResources { get; set; }

        public DbSet<QualificationIdentityResource> QualificationIdentityResources { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ProjectQualification>().HasKey(x => new { x.ProjectId, x.QualificationId });
            modelBuilder.Entity<ProjectQualification>().HasOne(x => x.Project).WithMany(x => x.ProjectQualifications).HasForeignKey(x => x.ProjectId);
            modelBuilder.Entity<ProjectQualification>().HasOne(x => x.Qualification).WithMany(x => x.ProjectQualification).HasForeignKey(x => x.QualificationId);

            modelBuilder.Entity<ProjectIdentityResource>().HasKey(x => new { x.ProjectId, ResourceId = x.IdentityResourceId });
            modelBuilder.Entity<ProjectIdentityResource>().HasOne(x => x.Project).WithMany(x => x.ProjectResources).HasForeignKey(x => x.ProjectId);
            modelBuilder.Entity<ProjectIdentityResource>().HasOne(x => x.IdentityResource).WithMany(x => x.ProjectResources).HasForeignKey(x => x.IdentityResourceId);

            modelBuilder.Entity<QualificationIdentityResource>().HasKey(x => new { x.QualificationId, ResourceId = x.IdentityResourceId });
            modelBuilder.Entity<QualificationIdentityResource>().HasOne(x => x.Qualification).WithMany(x => x.QualificationResources).HasForeignKey(x => x.QualificationId);
            modelBuilder.Entity<QualificationIdentityResource>().HasOne(x => x.IdentityResource).WithMany(x => x.QualificationResources).HasForeignKey(x => x.IdentityResourceId);
        }
    }
}