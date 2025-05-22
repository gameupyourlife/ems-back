using AutoMapper;
using ems_back.Repo.Data;
using ems_back.Repo.DTOs.Email;
using ems_back.Repo.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;

namespace ems_back.Repo.Repository
{
    public class EmailRepository : IEmailRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public EmailRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        public Task<EmailDto> CreateEventMail(Guid orgId, Guid eventId, EmailDto emailDto)
        {
            throw new NotImplementedException();
        }

        public Task<EmailDto> CreateMailTemplate(Guid orgId, EmailDto emailDto)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<EmailDto>> GetEventMails(Guid orgId, Guid eventId)
        {
			//var mails = await _context.Mail
			//    .Where(m => m.EventId == eventId)
			//    .Select(m => new EmailDto
			//    {
			//        MailId = m.MailId,
			//        Subject = m.Subject,
			//        Body = m.Body,
			//        Recipients = m.Recipients,
			//        ScheduledFor = m.ScheduledFor,
			//        CreatedAt = m.CreatedAt,
			//        UpdatedAt = m.UpdatedAt,
			//        CreatedBy = m.CreatedBy,
			//        UpdatedBy = m.UpdatedBy,

			//    })
			//    .AsNoTracking()
			//    .ToListAsync();
			//return mails;
			throw new NotImplementedException();
        }

        public Task<IEnumerable<EmailDto>> GetMailTemplates(Guid orgId)
        {
	        throw new NotImplementedException();
        }
    }
}