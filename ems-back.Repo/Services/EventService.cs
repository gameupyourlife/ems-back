using ems_back.Repo.Interfaces.Repository;

using ems_back.Repo.DTOs.Event;
using ems_back.Repo.Interfaces;
using ems_back.Repo.Models.Types;
using Microsoft.Extensions.Logging;
using ems_back.Repo.DTOs.File;
using Minio.DataModel.Args;
using ems_back.Repo.DTOs;
using Minio;
using Minio.Exceptions;

namespace ems_back.Repo.Services
{	public class EventService : IEventService
	{
		private readonly IMinioClient _minioClient;
		private readonly IEventRepository _eventRepository;
		private readonly IFileRepository _fileRepository;
		private readonly ILogger<EventService> _logger;
		private const string BucketName = "newest-test-created-bucket";

		public EventService(
			IEventRepository eventRepository,
			ILogger<EventService> logger)
		{
			_eventRepository = eventRepository;
			_logger = logger;
		}

		public async Task<IEnumerable<EventInfoDTO>> GetAllEventsAsync()
		{
			return await _eventRepository.GetAllEventsAsync();
		}

		public async Task<IEnumerable<EventInfoDTO>> GetUpcomingEventsAsync(int days = 30)
		{
			return await _eventRepository.GetUpcomingEventsAsync(days);
		}

		public async Task<EventInfoDTO> GetEventByIdAsync(Guid id)
		{
			var eventEntity = await _eventRepository.GetByIdAsync(id);
			if (eventEntity == null)
			{
				_logger.LogWarning("Event with id {EventId} not found", id);
			}
			return eventEntity;
		}

		public async Task<EventInfoDTO> GetEventWithAttendeesAsync(Guid id)
		{
			return await _eventRepository.GetEventWithAttendeesAsync(id);
		}

		public async Task<EventInfoDTO> GetEventWithAgendaAsync(Guid id)
		{
			return await _eventRepository.GetEventWithAgendaAsync(id);
		}

		public async Task<EventInfoDTO> GetEventWithAllDetailsAsync(Guid id)
		{
			return await _eventRepository.GetEventWithAllDetailsAsync(id);
		}

		public async Task<IEnumerable<EventInfoDTO>> GetEventsByOrganizationAsync(Guid organizationId)
		{
			return await _eventRepository.GetEventsByOrganizationAsync(organizationId);
		}

		public async Task<IEnumerable<EventInfoDTO>> GetEventsByCreatorAsync(Guid userId)
		{
			return await _eventRepository.GetEventsByCreatorAsync(userId);
		}

		public async Task<IEnumerable<EventInfoDTO>> GetEventsByCategoryAsync(int category)
		{
			return await _eventRepository.GetEventsByCategoryAsync(category);
		}

		public async Task<IEnumerable<EventInfoDTO>> GetEventsByDateRangeAsync(DateTime start, DateTime end)
		{
			return await _eventRepository.GetEventsByDateRangeAsync(start, end);
		}

		public async Task<EventInfoDTO> CreateEventAsync(EventCreateDto eventDto)
		{
			return await _eventRepository.AddAsync(eventDto);
		}

		public async Task<bool> UpdateEventAsync(Guid id, EventInfoDTO eventDto)
		{
			if (id != eventDto.Id)
			{
				return false;
			}
			return await _eventRepository.UpdateAsync(eventDto) != null;
		}

		public async Task<EventInfoDTO> UpdateEventStatusAsync(Guid id, EventInfoDTO statusDto)
		{
			return await _eventRepository.UpdateStatusAsync(id, statusDto);
		}

		public async Task<bool> DeleteEventAsync(Guid id)
		{
			return await _eventRepository.DeleteAsync(id);
		}

		public async Task<int> GetAttendeeCountAsync(Guid id)
		{
			return await _eventRepository.GetAttendeeCountAsync(id);
		}

		public async Task<FileDetailedDto> GetFileByIdAsync(string id)
		{
			return await _fileRepository.GetByIdAsync(id);
		}

		public async Task<IEnumerable<FileDto>> GetFilesByUserAsync(Guid userId)
		{
			return await _fileRepository.GetByUserAsync(userId);
		}

		public async Task<IEnumerable<FileDto>> GetFilesByTypeAsync(FileType type)
		{
			return await _fileRepository.GetByTypeAsync(type);
		}

		public async Task<FileDetailedDto> UploadFileAsync(FileUploadDto fileForm)
		{
			var file = fileForm.File;
			if (file == null || file.Length == 0)
			{
				throw new ArgumentException("No file uploaded.");
			}

			await EnsureBucketExistsAsync();

			var objectName = file.FileName;
			var contentType = file.ContentType;
			string etag;

			using (var stream = file.OpenReadStream())
			{
				var putObjectArgs = new PutObjectArgs()
					.WithBucket(BucketName)
					.WithObject(objectName)
					.WithStreamData(stream)
					.WithObjectSize(file.Length)
					.WithContentType(contentType);
				var res = await _minioClient.PutObjectAsync(putObjectArgs);
				etag = res.Etag.Trim('"');
			}

			var fileUrl = $"https://minio.gameup.dev/{BucketName}/{objectName}";

			var fileDto = new FileCreateDto
			{
				Url = fileUrl,
				ContentType = file.ContentType,
				Type = FileType.Document,
				SizeInBytes = file.Length,
				OriginalName = file.FileName,
				UploadedBy = new Guid("39eb0fb8-5ff3-4f1e-bfda-13ed1da832ed"),
				Id = etag
			};

			return await _fileRepository.AddAsync(fileDto);
		}

		public async Task<bool> DeleteFileAsync(string id)
		{
			var metadata = await _fileRepository.GetByIdAsync(id);
			if (metadata == null) return false;

			try
			{
				await _minioClient.RemoveObjectAsync(new RemoveObjectArgs()
					.WithBucket(BucketName)
					.WithObject(metadata.OriginalName));
			}
			catch (MinioException e)
			{
				_logger.LogError(e, "Error deleting file from Minio");
				throw;
			}

			return await _fileRepository.DeleteAsync(id);
		}

		public async Task<int> GetFileCountByUserAsync(Guid userId)
		{
			return await _fileRepository.GetCountByUserAsync(userId);
		}

		private async Task EnsureBucketExistsAsync()
		{
			var beArgs = new BucketExistsArgs().WithBucket(BucketName);
			bool found = await _minioClient.BucketExistsAsync(beArgs);

			if (!found)
			{
				var mbArgs = new MakeBucketArgs().WithBucket(BucketName);
				await _minioClient.MakeBucketAsync(mbArgs);

				var policyJson = $@"{{
                    ""Version"": ""2012-10-17"",
                    ""Statement"": [
                        {{
                            ""Effect"": ""Allow"",
                            ""Principal"": {{""AWS"": ""*""}},
                            ""Action"": [""s3:GetObject""],
                            ""Resource"": [""arn:aws:s3:::{BucketName}/*""]
                        }}
                    ]
                }}";

				var args = new SetPolicyArgs()
					.WithBucket(BucketName)
					.WithPolicy(policyJson);

				await _minioClient.SetPolicyAsync(args);
			}
		}
	}
}
