using AutoMapper;
using ems_back.Repo.Data;
using ems_back.Repo.DTOs.Email;
using ems_back.Repo.Exceptions;
using ems_back.Repo.Interfaces.Repository;
using ems_back.Repo.Models;
using Microsoft.AspNetCore.Http.HttpResults;
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

        public async Task<MailDto> GetMailByIdAsync(Guid orgId, Guid eventId, Guid mailId)
        {
            var mail = await _context.Mail
                .Where(m => m.EventId == eventId && m.Id == mailId)
                .Select(m => new MailDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    Subject = m.Subject,
                    Body = m.Body,
                    Description = m.Description,
                    Recipients = m.Recipients,
                    ScheduledFor = m.ScheduledFor,
                    CreatedBy = m.CreatedBy,
                    UpdatedBy = m.UpdatedBy,
                    CreatedAt = m.CreatedAt,
                    UpdatedAt = m.UpdatedAt,
                    sendToAllParticipants = m.sendToAllParticipants,
                    IsUserCreated = m.IsUserCreated,
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (mail == null)
            {
                throw new NotFoundException("Mail not found");
            }

                return mail;
        }

        public async Task<Guid> CreateMailAsync(Guid eventId, MailDto mail)
        {
            var newMail = new Mail
            {
                Name = mail.Name,
                Subject = mail.Subject,
                Body = mail.Body,
                CreatedAt = mail.CreatedAt,
                UpdatedAt = mail.UpdatedAt,
                sendToAllParticipants = mail.sendToAllParticipants,
                IsUserCreated = mail.IsUserCreated,
                EventId = eventId,
                CreatedBy = mail.CreatedBy,
                UpdatedBy = mail.UpdatedBy,
                Recipients = mail.Recipients,
                ScheduledFor = mail.ScheduledFor,
                Description = mail.Description
            };

            _context.Mail.Add(newMail);
            await _context.SaveChangesAsync();

            return newMail.Id;
        }

        public async Task<bool> DeleteMailAsync(Guid orgId, Guid eventId, Guid mailId)
        {
            var mail = await _context.Mail
                .Where(m => m.EventId == eventId && m.Id == mailId)
                .FirstOrDefaultAsync();

            if (mail == null)
            {
                return false;
            }

            _context.Mail.Remove(mail);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<MailDto> UpdateMailAsync(
            Guid orgId, 
            Guid eventId, 
            Guid mailId, 
            CreateMailDto mailDto, 
            Guid userId)
        {
            var existingMail = await _context.Mail
                .Where(m => m.EventId == eventId && m.Id == mailId)
                .FirstOrDefaultAsync();
            if (existingMail == null)
            {
                throw new NotFoundException("Mail not found");
            }

            _mapper.Map(mailDto, existingMail);
            existingMail.UpdatedAt = DateTime.UtcNow;
            existingMail.UpdatedBy = userId;

            _context.Mail.Update(existingMail);
            await _context.SaveChangesAsync();

            return await GetMailByIdAsync(orgId, eventId, mailId);
        }
    }
}