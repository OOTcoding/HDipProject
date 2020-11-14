using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PM.MVC.Models.EF;
using PM.MVC.Models.Exceptions;
using PM.MVC.Models.Interfaces;

namespace PM.MVC.Models.Services
{
    public class IdentityResourceQualificationService : IQualificationService<IdentityResource>
    {
        private readonly PMAppDbContext _context;

        public IdentityResourceQualificationService(PMAppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Qualification>> GetSuitableQualificationsAsync(IdentityResource source)
        {
            var resourceQualifications = await _context.QualificationIdentityResources.Include(x => x.Qualification)
                                                       .ThenInclude(x => x.Skill)
                                                       .Where(x => x.IdentityResourceId == source.Id)
                                                       .ToArrayAsync();
            var qualifications = await _context.Qualifications.ToArrayAsync();

            return qualifications.Where(qualification => resourceQualifications.All(p => p.QualificationId != qualification.Id));
        }

        public async Task AddQualificationsAsync(IdentityResource source, IEnumerable<int> qualificationsToAddIds)
        {
            foreach (int qualificationId in qualificationsToAddIds)
            {
                QualificationIdentityResource qualificationIdentityResource =
                    new QualificationIdentityResource { IdentityResourceId = source.Id, QualificationId = qualificationId };
                await _context.QualificationIdentityResources.AddAsync(qualificationIdentityResource);
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteQualificationAsync(IdentityResource source, int qualificationId)
        {
            QualificationIdentityResource qualificationIdentityResource = await GetDbResourceQualification(source.Id, qualificationId);

            _context.QualificationIdentityResources.Remove(qualificationIdentityResource);
            await _context.SaveChangesAsync();
        }

        private async Task<QualificationIdentityResource> GetDbResourceQualification(string resourceId, int qualificationId)
        {
            var qualificationIdentityResource = await _context.QualificationIdentityResources.Include(x => x.IdentityResource)
                                                              .Include(x => x.Qualification)
                                                              .FirstOrDefaultAsync(x => x.QualificationId == qualificationId && x.IdentityResourceId == resourceId);

            if (qualificationIdentityResource == null)
            {
                throw new RequestedResourceNotFoundException();
            }

            return qualificationIdentityResource;
        }
    }
}