using ems_back.Repo.DTOs;
using ems_back.Repo.DTOs.File;
using ems_back.Repo.Interfaces;
using ems_back.Repo.Models.Types;
using Microsoft.AspNetCore.Mvc;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;

[Route("api/[controller]")]
[ApiController]
public class FilesController : ControllerBase
{
    private readonly IFileRepository _fileRepository;
    private readonly ILogger<FilesController> _logger;
    private readonly IMinioClient _minioClient;

    public FilesController(
        IFileRepository fileRepository,
        IMinioClient minioClient,
        ILogger<FilesController> logger)
    {
        _fileRepository = fileRepository;
        _logger = logger;
        _minioClient = minioClient;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<FileDetailedDto>> GetFile(string id)
    {
        try
        {
            var file = await _fileRepository.GetByIdAsync(id);
            if (file == null) return NotFound();
            return Ok(file);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting file with id {FileId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<FileDto>>> GetFilesByUser(Guid userId)
    {
        try
        {
            var files = await _fileRepository.GetByUserAsync(userId);
            return Ok(files);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting files for user {UserId}", userId);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("type/{type}")]
    public async Task<ActionResult<IEnumerable<FileDto>>> GetFilesByType(FileType type)
    {
        try
        {
            var files = await _fileRepository.GetByTypeAsync(type);
            return Ok(files);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting files of type {FileType}", type);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    public async Task<ActionResult<FileDetailedDto>> UploadFile([FromForm] FileUploadDto fileForm)
    {
        try
        {
            var file = fileForm.File;

            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var bucketName = "newest-test-created-bucket";
            var objectName = file.FileName;
            var contentType = file.ContentType;
            string? etag = null;
            try
            {
                // Make a bucket on the server, if not already present.
                var beArgs = new BucketExistsArgs()
                    .WithBucket(bucketName);
                bool found = await _minioClient.BucketExistsAsync(beArgs).ConfigureAwait(false);
                if (!found)
                {
                    var mbArgs = new MakeBucketArgs()
                        .WithBucket(bucketName);

                    var policyJson = $@"
						 {{
						""Version"": ""2012-10-17"",
						""Statement"": [
							{{
								""Effect"": ""Allow"",
								""Principal"": {{""AWS"": ""*""}},
								""Action"": [
									""s3:GetObject""
								],
								""Resource"": [
									""arn:aws:s3:::{bucketName}/*""
								]	
							}}
						]
						 }}";

                    var args = new SetPolicyArgs()
                        .WithBucket(bucketName)
                        .WithPolicy(policyJson);


                    await _minioClient.MakeBucketAsync(mbArgs).ConfigureAwait(false);
                    await _minioClient.SetPolicyAsync(args).ConfigureAwait(false);
                }

                // Upload the file to the bucket.
                using (var stream = file.OpenReadStream())
                {
                    var putObjectArgs = new PutObjectArgs()
                        .WithBucket(bucketName)
                        .WithObject(objectName)
                        .WithStreamData(stream)
                        .WithObjectSize(file.Length)
                        .WithContentType(contentType);
                    var res = await _minioClient.PutObjectAsync(putObjectArgs).ConfigureAwait(false);

                    etag = res.Etag.Trim('"'); // For some reason the tag is returned with quotes
                    _logger.LogInformation("File uploaded successfully with ETag: {Etag}", etag);
                }

                _logger.LogInformation("Successfully uploaded {ObjectName}", objectName);
            }
            catch (MinioException e)
            {
                _logger.LogError(e, "File Upload Error: {Message}", e.Message);
                return StatusCode(500, "Error uploading file to Minio");
            }

            var fileUrl = $"https://minio.gameup.dev/{bucketName}/{objectName}";


            // Optionally, save file metadata to the repository
            var fileDto = new FileCreateDto
            {
                Url = fileUrl,
                ContentType = file.ContentType,
                Type = FileType.Document, // Set the appropriate file type
                SizeInBytes = file.Length,
                OriginalName = file.FileName,
                UploadedBy = new Guid("39eb0fb8-5ff3-4f1e-bfda-13ed1da832ed"),
                Id = etag

                // Weitere Eigenschaften setzen, falls erforderlich
            };

            var createdFile = await _fileRepository.AddAsync(fileDto);
            return CreatedAtAction(
                nameof(GetFile),
                new { id = etag },
                createdFile);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateFile(string id, [FromBody] FileUpdateDto fileDto)
    {
        // Deprecated: This method is not used in the current implementation.
        return NotFound("This method is deprecated and not used in the current implementation.");

        try
        {
            if (id != fileDto.Id) return BadRequest("ID mismatch");

            var updatedFile = await _fileRepository.UpdateAsync(fileDto);
            if (updatedFile == null) return NotFound();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating file with id {FileId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFile(string id)
    {
        try
        {
            var metadata = await _fileRepository.GetByIdAsync(id);

            try
            {
                await _minioClient.RemoveObjectAsync(new RemoveObjectArgs()
                    .WithBucket("newest-test-created-bucket")
                    .WithObject(metadata.OriginalName));
            }

            catch (MinioException e)
            {
                return StatusCode(500, "Error deleting file from Minio");
            }

            var result = await _fileRepository.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file with id {FileId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("user/{userId}/count")]
    public async Task<ActionResult<int>> GetFileCountByUser(Guid userId)
    {
        try
        {
            var count = await _fileRepository.GetCountByUserAsync(userId);
            return Ok(count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting file count for user {UserId}", userId);
            return StatusCode(500, "Internal server error");
        }
    }
}