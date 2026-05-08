using Microsoft.EntityFrameworkCore;
using UrbanFix.Common.Pagination;
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

        public async Task<PaginationResponse<Verification>> GetAllVerificationsAsync(int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.Verifications;
            var totalCount = await query.CountAsync();

            var verifications = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginationResponse<Verification>
            {
                Items = verifications,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task UpdateAsync(Verification verification)
        {
            _context.Verifications.Update(verification);
            await _context.SaveChangesAsync();
        }
    }
}
