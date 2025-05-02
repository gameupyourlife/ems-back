// Services/FileService.cs
using ems_back.Repo.DTOs;
using ems_back.Repo.DTOs.File;
using ems_back.Repo.Interfaces;
using ems_back.Repo.Models.Types;
using ems_back.Repo.Services.Interfaces;

using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ems_back.Services
{
	public class FileService : IFileService
	{
		private readonly IFileRepository _fileRepository;
		private readonly IMinioClient _minioClient;
		private readonly ILogger<FileService> _logger;
		private const string BucketName = "newest-test-created-bucket";

		public FileService(
			IFileRepository fileRepository,
			IMinioClient minioClient,
			ILogger<FileService> logger)
		{
			_fileRepository = fileRepository;
			_minioClient = minioClient;
			_logger = logger;
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