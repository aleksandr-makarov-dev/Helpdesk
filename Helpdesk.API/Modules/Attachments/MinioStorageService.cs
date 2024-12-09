using FluentResults;
using Minio;
using Minio.DataModel.Args;

namespace Helpdesk.API.Modules.Attachments
{
    public class MinioStorageService:IStorageService
    {
        private readonly IMinioClient _minioClient;
        private const string BucketName = "mybucket";

        public MinioStorageService(IMinioClient minioClient)
        {
            _minioClient = minioClient;
        }

        public async Task<Result<string>> StoreAsync(IFormFile file)
        {
            // TODO: check if bucket exists

            try
            {
                var bucketExistsResult =
                    await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(BucketName));

                if (!bucketExistsResult)
                {
                    return Result.Fail(new Error($"Bucked {BucketName} does not exist"));
                }

                var pubObjectResult = await _minioClient.PutObjectAsync(
                    new PutObjectArgs()
                        .WithBucket(BucketName)
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
                        .WithBucket(BucketName)
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
