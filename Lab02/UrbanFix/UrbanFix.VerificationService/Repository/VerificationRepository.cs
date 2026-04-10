using Microsoft.EntityFrameworkCore;
using UrbanFix.VerificationService.Models;

namespace UrbanFix.VerificationService.Repository
{
    public class VerificationRepository : IVerificationRepository
    {
        private readonly VerificationDbContext _context;
        public VerificationRepository(VerificationDbContext context) => _context = context;

        public async Task AddAsync(Verification verification)
        {
            await _context.Verifications.AddAsync(verification);
            await _context.SaveChangesAsync();
        }

        public async Task<Verification?> GetByReportIdAsync(Guid reportId)
        {
            return await _context.Verifications.FirstOrDefaultAsync(v => v.ReportId == reportId);
        }

        public async Task UpdateAsync(Verification verification)
        {
            _context.Verifications.Update(verification);
            await _context.SaveChangesAsync();
        }
    }
}
