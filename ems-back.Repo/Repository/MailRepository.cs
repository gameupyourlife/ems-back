using AutoMapper;
using ems_back.Repo.Data;
using ems_back.Repo.DTOs.Email;
using ems_back.Repo.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;

namespace ems_back.Repo.Repository
{
    public class MailRepository : IMailRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public MailRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public Task<bool> DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<MailDto>> GetMailsForEventAsync(Guid orgId, Guid eventId)
        {
            var mails = await _context.Mail
                .Where(m => m.EventId == eventId)
                .Select(m => new MailDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    Subject = m.Subject,
                    Body = m.Body,
                    CreatedAt = m.CreatedAt,
                    UpdatedAt = m.UpdatedAt,
                    sendToAllParticipants = m.sendToAllParticipants,
                    IsUserCreated = m.IsUserCreated,
                })
                .AsNoTracking()
                .ToListAsync();

            return mails;
        }
    }
}