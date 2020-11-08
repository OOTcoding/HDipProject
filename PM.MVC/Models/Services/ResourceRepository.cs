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
    public class ResourceRepository : IDisposable, IQualificationRepository<IdentityResource>
    {
        private readonly PMAppDbContext _context;

        public ResourceRepository(PMAppDbContext context)
        {
            _context = context;
        }

        public async Task<IdentityResource> AddQualificationAsync(IdentityResource dbIdentityResource, Qualification createRequest)
        {
            Qualification dbQualification = await _context.Qualifications.FirstOrDefaultAsync(x => x.Skill.Name == createRequest.Skill.Name && x.Level == createRequest.Level);

            if (dbQualification == null)
            {
                Skill dbSkill = await GetDbSkill(createRequest.Skill);

                dbQualification = new Qualification { Skill = dbSkill, Level = createRequest.Level };
            }

            dbIdentityResource.QualificationResources.Add(new QualificationIdentityResource
            {
                Qualification = dbQualification, QualificationId = dbQualification.Id, IdentityResource = dbIdentityResource, IdentityResourceId = dbIdentityResource.Id
            });
            await _context.SaveChangesAsync();

            return dbIdentityResource;
        }

        public async Task DeleteQualificationAsync(IdentityResource dbIdentityResource, int qualificationId)
        {
            var dbQualification = dbIdentityResource.QualificationResources.FirstOrDefault(x => x.QualificationId == qualificationId);

            if (dbQualification == null)
            {
                throw new RequestedResourceNotFoundException();
            }

            dbIdentityResource.QualificationResources.Remove(dbQualification);
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        private async Task<Skill> GetDbSkill(Skill createRequest)
        {
            var dbSkill = await _context.Skills.FirstOrDefaultAsync(x => x.Name == createRequest.Name);

            return dbSkill ?? (await _context.Skills.AddAsync(new Skill { Name = createRequest.Name })).Entity;
        }
    }
}