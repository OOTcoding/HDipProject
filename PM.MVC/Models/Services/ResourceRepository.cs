using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PM.MVC.Models.EF;
using PM.MVC.Models.Exceptions;
using PM.MVC.Models.Services.Interfaces;

namespace PM.MVC.Models.Services
{
    public class ResourceRepository : IDisposable, IQualificationRepository<Resource>
    {
        private readonly PMAppDbContext _context;

        public ResourceRepository(PMAppDbContext context)
        {
            _context = context;
        }

        public async Task<Resource> GetOneAsync(int id)
        {
            return await GetDbResource(id);
        }

        public async Task<IEnumerable<Resource>> GetAllAsync()
        {
            var dbResources = await _context.Resources.Include(x => x.ProjectResources).Include(x => x.QualificationResources).ThenInclude(x => x.Qualification).ToArrayAsync();

            await _context.Skills.LoadAsync();

            return dbResources;
        }

        public async Task<Resource> AddQualificationAsync(int resourceId, Qualification createRequest)
        {
            Resource dbResource = await GetDbResource(resourceId);

            Qualification dbQualification = await _context.Qualifications.FirstOrDefaultAsync(x => x.Skill.Name == createRequest.Skill.Name && x.Level == createRequest.Level);

            if (dbQualification == null)
            {
                Skill dbSkill = await GetDbSkill(createRequest.Skill);

                dbQualification = new Qualification { Skill = dbSkill, Level = createRequest.Level };
            }

            dbResource.QualificationResources.Add(new QualificationResource
            {
                Qualification = dbQualification, QualificationId = dbQualification.Id, Resource = dbResource, ResourceId = resourceId
            });
            await _context.SaveChangesAsync();

            return dbResource;
        }

        public async Task DeleteQualificationAsync(int resourceId, int qualificationId)
        {
            Resource dbResource = await GetDbResource(resourceId);

            var dbQualification = dbResource.QualificationResources.FirstOrDefault(x => x.QualificationId == qualificationId);

            if (dbQualification == null)
            {
                throw new RequestedResourceNotFoundException();
            }

            dbResource.QualificationResources.Remove(dbQualification);
            await _context.SaveChangesAsync();
        }

        public async Task<Resource> AddAsync(Resource createRequest)
        {
            var dbResource = (await _context.Resources.AddAsync(createRequest)).Entity;
            await _context.SaveChangesAsync();

            return dbResource;
        }

        public async Task<Resource> UpdateAsync(Resource updateRequest)
        {
            Resource dbResource = await GetDbResource(updateRequest.Id);
            dbResource.Name = updateRequest.Name;
            _context.Entry(dbResource).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return dbResource;
        }

        public async Task DeleteAsync(int id)
        {
            Resource dbResource = await GetDbResource(id);

            _context.Resources.Remove(dbResource);
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        private async Task<Resource> GetDbResource(int resourceId)
        {
            Resource dbResource = await _context.Resources.Include(x => x.ProjectResources)
                                                .Include(x => x.QualificationResources)
                                                .ThenInclude(x => x.Qualification)
                                                .FirstOrDefaultAsync(x => x.Id == resourceId);

            if (dbResource == null)
            {
                throw new RequestedResourceNotFoundException();
            }

            await _context.Skills.LoadAsync();
            return dbResource;
        }

        private async Task<Skill> GetDbSkill(Skill createRequest)
        {
            var dbSkill = await _context.Skills.FirstOrDefaultAsync(x => x.Name == createRequest.Name);

            return dbSkill ?? (await _context.Skills.AddAsync(new Skill { Name = createRequest.Name })).Entity;
        }
    }
}