using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PM.MVC.Models.EF;
using PM.MVC.Models.Exceptions;
using PM.MVC.Models.Interfaces;

namespace PM.MVC.Models.Repositories
{
    public class ProjectRepository : IDisposable, IRepository<Project>
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
            var projects = await _context.Projects.Include(x => x.Manager).Include(x => x.ProjectQualifications).Include(x => x.ProjectResources).ToArrayAsync();
            await _context.Skills.LoadAsync();
            return projects;
        }

        public async Task<Project> AddAsync(Project createRequest)
        {
            var dbProjects = await _context.Projects.Where(x => x.Name == createRequest.Name).ToArrayAsync();

            if (dbProjects.Length > 0)
            {
                throw new RequestedResourceHasConflictException();
            }

            var dbProject = await _context.Projects.AddAsync(createRequest);
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
            dbProject.FromDuration = updateRequest.FromDuration;
            dbProject.ToDuration = updateRequest.ToDuration;
            dbProject.ManagerId = updateRequest.ManagerId ?? dbProject.ManagerId;

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

        public void Dispose()
        {
            _context?.Dispose();
        }

        private async Task<Project> GetDbProject(int id)
        {
            Project dbProject = await _context.Projects.Include(x => x.Manager)
                                              .Include(x => x.ProjectQualifications)
                                              .ThenInclude(x => x.Qualification)
                                              .Include(x => x.ProjectResources)
                                              .ThenInclude(x => x.IdentityResource)
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
    }
}