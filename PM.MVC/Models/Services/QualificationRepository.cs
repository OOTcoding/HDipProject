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
    public class QualificationRepository : IDisposable, IRepository<Qualification>
    {
        private readonly PMAppDbContext _context;

        public QualificationRepository(PMAppDbContext context)
        {
            _context = context;
        }

        public async Task<Qualification> GetOneAsync(int id)
        {
            Qualification dbQualification = await GetDbQualification(id);

            return dbQualification;
        }

        public async Task<IEnumerable<Qualification>> GetAllAsync()
        {
            var dbQualifications = await _context.Qualifications.Include(x => x.Skill).ToArrayAsync();

            return dbQualifications;
        }

        public async Task<Qualification> AddAsync(Qualification createRequest)
        {
            Qualification dbQualification = await _context.Qualifications.FirstOrDefaultAsync(x => x.Skill.Name == createRequest.Skill.Name && x.Level == createRequest.Level);

            if (dbQualification != null)
            {
                throw new RequestedResourceHasConflictException();
            }

            Skill dbSkill = await GetDbSkill(createRequest.Skill);

            dbQualification = (await _context.Qualifications.AddAsync(new Qualification { Skill = dbSkill, Level = createRequest.Level })).Entity;
            await _context.SaveChangesAsync();

            return dbQualification;
        }

        public async Task<Qualification> UpdateAsync(Qualification updateRequest)
        {
            Qualification dbQualification = await GetDbQualification(updateRequest.Id);
            Skill dbSkill = await GetDbSkill(updateRequest.Skill);

            dbQualification.Skill = dbSkill;
            dbQualification.Level = updateRequest.Level;

            await _context.SaveChangesAsync();

            return dbQualification;
        }

        public async Task DeleteAsync(int id)
        {
            var dbQualifications = await _context.Qualifications.Where(x => x.Id == id).ToArrayAsync();

            if (dbQualifications.Length == 0)
            {
                throw new RequestedResourceNotFoundException();
            }

            var dbQualification = dbQualifications[0];

            _context.Qualifications.Remove(dbQualification);
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

        private async Task<Qualification> GetDbQualification(int id)
        {
            Qualification dbQualification =
                await _context.Qualifications.Include(x => x.ProjectQualification).Include(x => x.Skill).Include(x => x.QualificationResources).FirstOrDefaultAsync(x => x.Id == id);

            if (dbQualification == null)
            {
                throw new RequestedResourceNotFoundException();
            }

            return dbQualification;
        }
    }
}