﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PM.MVC.Models.EF;
using PM.MVC.Models.Exceptions;
using PM.MVC.Models.Services.Interfaces;

namespace PM.MVC.Models.Services
{
    public class ProjectRepository: IDisposable, IProjectRepository
    {
        private readonly PMAppDbContext _context;

        public ProjectRepository(PMAppDbContext context)
        {
            _context = context;
        }

        public async Task<Project> GetOneAsync(int id)
        {
            Project dbProject = await GetDbProject(id);
            return dbProject;
        }

        public async Task<IEnumerable<Project>> GetAllAsync()
        {
            var projects = await _context.Projects.Include(x => x.ProjectQualifications).Include(x => x.ProjectResources).ToArrayAsync();
            await _context.Skills.LoadAsync();
            return projects;
        }

        public async Task<IEnumerable<Resource>> GetSuitableResourcesAsync(int projectId)
        {
            Project dbProject = await GetDbProject(projectId);

            Resource[] resources = await _context.Resources.Include(x => x.ProjectResources)
                                     .ThenInclude(x => x.Resource)
                                     .Include(x => x.QualificationResources)
                                     .ThenInclude(x => x.Qualification)
                                     .ToArrayAsync();

            return resources
                   .Select(resource =>
                               (resource,
                                meetRequirementsQualifications: resource.QualificationResources
                                                                        .Where(x => dbProject.ProjectQualifications.Any(d => d.QualificationId == x.QualificationId))
                                                                        .ToList()))
                   .Where(t => t.meetRequirementsQualifications.Count > 0 && t.resource.ProjectResources.All(x => x.ProjectId != projectId))
                   .Select(t => t.resource);
        }

        public async Task<IEnumerable<Qualification>> GetSuitableQualificationsAsync(int projectId)
        {
            Project dbProject = await GetDbProject(projectId);

            var qualifications = await _context.Qualifications.ToArrayAsync();

            return qualifications.Where(qualification => dbProject.ProjectQualifications.All(p => p.QualificationId != qualification.Id));
        }

        public async Task<Project> AddAsync(Project createRequest)
        {
            var dbProjects = await _context.Projects.Where(x => x.Name == createRequest.Name).ToArrayAsync();

            if (dbProjects.Length > 0)
            {
                throw new RequestedResourceHasConflictException();
            }

            var dbProject = await _context.Projects.AddAsync(new Project { Name = createRequest.Name });
            await _context.SaveChangesAsync();

            return dbProject.Entity;
        }

        public async Task<Project> UpdateAsync(Project updateRequest)
        {
            var dbProjects = await _context.Projects.Where(x => x.Name == updateRequest.Name && x.Id != updateRequest.Id).ToArrayAsync();

            if (dbProjects.Length > 0)
            {
                throw new RequestedResourceHasConflictException();
            }

            dbProjects = await _context.Projects.Where(x => x.Id == updateRequest.Id).ToArrayAsync();

            if (dbProjects.Length == 0)
            {
                throw new RequestedResourceNotFoundException();
            }

            var dbProject = dbProjects[0];

            dbProject.Name = updateRequest.Name;

            await _context.SaveChangesAsync();

            return dbProject;
        }

        public async Task DeleteAsync(int id)
        {
            var dbProjects = await _context.Projects.Where(x => x.Id == id).ToArrayAsync();

            if (dbProjects.Length == 0)
            {
                throw new RequestedResourceNotFoundException();
            }

            var dbProject = dbProjects[0];

            _context.Projects.Remove(dbProject);
            await _context.SaveChangesAsync();
        }

        public async Task<Project> AddResourcesAsync(int sourceId, IEnumerable<Resource> resourcesToAdd)
        {
            Project dbProject = await GetDbProject(sourceId);

            Resource[] query = await _context.Resources.Include(x => x.ProjectResources).ToArrayAsync();
            var resources = query.Where(x => resourcesToAdd.Any(r => r.Id == x.Id));

            foreach (Resource resource in resources)
            {
                ProjectResource projectResource = new ProjectResource { Project = dbProject, ProjectId = dbProject.Id, Resource = resource, ResourceId = resource.Id };
                dbProject.ProjectResources.Add(projectResource);
                resource.ProjectResources.Add(projectResource);
            }

            await _context.SaveChangesAsync();

            return dbProject;
        }

        public async Task<Project> AddQualificationsAsync(int projectId, IEnumerable<Qualification> qualificationsToAdd)
        {
            Project dbProject = await GetDbProject(projectId);

            Qualification[] query = await _context.Qualifications.Include(x => x.ProjectQualification).ToArrayAsync();
            var qualifications = query.Where(x => qualificationsToAdd.Any(q => q.Id == x.Id));

            foreach (Qualification qualification in qualifications)
            {
                ProjectQualification projectQualification = new ProjectQualification
                {
                    Project = dbProject, ProjectId = projectId, Qualification = qualification, QualificationId = qualification.Id
                };
                dbProject.ProjectQualifications.Add(projectQualification);
                qualification.ProjectQualification.Add(projectQualification);
            }

            await _context.SaveChangesAsync();

            return dbProject;
        }

        public async Task<Project> AddQualificationAsync(int id, Qualification createRequest)
        {
            Project dbProject = await GetDbProject(id);

            Qualification[] query = await _context.Qualifications.Include(x => x.ProjectQualification).ToArrayAsync();
            var qualification = query.FirstOrDefault(q => q.Id == createRequest.Id);

            if (qualification != null)
            {
                ProjectQualification projectQualification = new ProjectQualification
                {
                    Project = dbProject, ProjectId = id, Qualification = qualification, QualificationId = qualification.Id
                };
                dbProject.ProjectQualifications.Add(projectQualification);
                qualification.ProjectQualification.Add(projectQualification);
            }

            return dbProject;
        }

        public async Task DeleteQualificationAsync(int projectId, int qualificationId)
        {
            ProjectQualification projectQualification = await GetDbProjectQualification(projectId, qualificationId);

            _context.ProjectQualifications.Remove(projectQualification);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteResourceAsync(int sourceId, int resourceId)
        {
            ProjectResource projectResource = await GetDbProjectResource(sourceId, resourceId);

            _context.ProjectResources.Remove(projectResource);
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }

        private async Task<Project> GetDbProject(int id)
        {
            Project dbProject = await _context.Projects.Include(x => x.ProjectQualifications)
                                  .ThenInclude(x => x.Qualification)
                                  .Include(x => x.ProjectResources)
                                  .ThenInclude(x => x.Resource)
                                  .ThenInclude(x => x.QualificationResources)
                                  .ThenInclude(x => x.Qualification)
                                  .FirstOrDefaultAsync(x => x.Id == id);

            if (dbProject == null)
            {
                throw new RequestedResourceNotFoundException();
            }

            await _context.Skills.LoadAsync();

            return dbProject;
        }

        private async Task<ProjectQualification> GetDbProjectQualification(int projectId, int qualificationId)
        {
            var projectQualification = await _context.ProjectQualifications.Include(x => x.Project)
                                                     .Include(x => x.Qualification)
                                                     .FirstOrDefaultAsync(x => x.QualificationId == qualificationId && x.ProjectId == projectId);

            if (projectQualification == null)
            {
                throw new RequestedResourceNotFoundException();
            }

            return projectQualification;
        }

        private async Task<ProjectResource> GetDbProjectResource(int projectId, int resourceId)
        {
            var projectResource = await _context.ProjectResources.Include(x => x.Project)
                                                .Include(x => x.Resource)
                                                .FirstOrDefaultAsync(x => x.ProjectId == projectId && x.ResourceId == resourceId);

            if (projectResource == null)
            {
                throw new RequestedResourceNotFoundException();
            }

            return projectResource;
        }
    }
}