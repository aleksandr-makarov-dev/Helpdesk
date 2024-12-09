using FluentResults;
using Helpdesk.API.Configuration;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;

namespace Helpdesk.API.Modules.Attachments
{
    public class MinioStorageService:IStorageService
    {
        private readonly IMinioClient _minioClient;
        private readonly ApplicationOptions _applicationOptions;

        public MinioStorageService(IMinioClient minioClient, IOptions<ApplicationOptions> applicationOptions)
        {
            _minioClient = minioClient;
            _applicationOptions = applicationOptions.Value;
        }

        public async Task<Result<string>> StoreAsync(IFormFile file)
        {
            // TODO: check if bucket exists

            try
            {
                var bucketExistsResult =
                    await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(_applicationOptions.MinioBucketName));

                if (!bucketExistsResult)
                {
                    return Result.Fail(new Error($"Bucked {_applicationOptions.MinioBucketName} does not exist"));
                }

                var pubObjectResult = await _minioClient.PutObjectAsync(
                    new PutObjectArgs()
                        .WithBucket(_applicationOptions.MinioBucketName)
                        .WithObject(file.FileName)
                        .WithStreamData(file.OpenReadStream())
                        .WithObjectSize(file.Length)
                        .WithContentType(file.ContentType)
                );

                var preSignedUrl = await RetrieveAsync(pubObjectResult.ObjectName);

                return preSignedUrl is null
                    ? Result.Fail(new Error("Failed to generate url"))
                    : Result.Ok(preSignedUrl);
            }
            catch (Exception e)
            {
                return Result.Fail(new Error(e.Message));
            }
        }

        public async Task<string?> RetrieveAsync(string fileName)
        {
            try
            {
                return await _minioClient.PresignedGetObjectAsync(
                    new PresignedGetObjectArgs()
                        .WithBucket(_applicationOptions.MinioBucketName)
                        .WithObject(fileName)
                        .WithExpiry(86400)
                );

            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
