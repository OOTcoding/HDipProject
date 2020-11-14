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
    public class SkillRepository : IDisposable, IRepository<Skill>
    {
        private readonly PMAppDbContext _context;

        public SkillRepository(PMAppDbContext context)
        {
            _context = context;
        }

        public async Task<Skill> GetOneAsync(int id)
        {
            Skill dbSkill = await _context.Skills.FindAsync(id);

            if (dbSkill == null)
            {
                throw new RequestedResourceNotFoundException();
            }

            return dbSkill;
        }

        public async Task<IEnumerable<Skill>> GetAllAsync()
        {
            Skill[] skills = await _context.Skills.ToArrayAsync();

            return skills;
        }

        public async Task<Skill> AddAsync(Skill createRequest)
        {
            var dbSkills = await _context.Skills.Where(x => x.Name == createRequest.Name).ToArrayAsync();

            if (dbSkills.Length > 0)
            {
                throw new RequestedResourceHasConflictException();
            }

            var dbSkill = (await _context.Skills.AddAsync(new Skill { Name = createRequest.Name })).Entity;
            await _context.SaveChangesAsync();

            return dbSkill;
        }

        public async Task<Skill> UpdateAsync(Skill updateRequest)
        {
            var dbSkills = await _context.Skills.Where(x => x.Name == updateRequest.Name && x.Id != updateRequest.Id).ToArrayAsync();

            if (dbSkills.Length > 0)
            {
                throw new RequestedResourceHasConflictException();
            }

            dbSkills = await _context.Skills.Where(x => x.Id == updateRequest.Id).ToArrayAsync();

            if (dbSkills.Length == 0)
            {
                throw new RequestedResourceNotFoundException();
            }

            var dbSkill = dbSkills[0];

            dbSkill.Name = updateRequest.Name;

            await _context.SaveChangesAsync();

            return dbSkill;
        }

        public async Task DeleteAsync(int id)
        {
            var dbSkills = await _context.Skills.Where(x => x.Id == id).ToArrayAsync();

            if (dbSkills.Length == 0)
            {
                throw new RequestedResourceNotFoundException();
            }

            var dbSkill = dbSkills[0];

            _context.Skills.Remove(dbSkill);
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}