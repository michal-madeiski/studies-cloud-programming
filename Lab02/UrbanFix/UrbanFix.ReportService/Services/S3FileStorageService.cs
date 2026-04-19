using Amazon.S3;
using Amazon.S3.Model;

namespace UrbanFix.ReportService.Services
{
    public class S3FileStorageService : IS3FileStorageService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly IConfiguration _configuration;
        private readonly ILogger<S3FileStorageService> _logger;

        public S3FileStorageService(
            IAmazonS3 s3Client,
            IConfiguration configuration,
            ILogger<S3FileStorageService> logger)
        {
            _s3Client = s3Client;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<(string BucketName, string ObjectKey, long FileSize)> UploadFileAsync(
            Stream fileStream,
            string fileName,
            string contentType)
        {
            _logger.LogInformation($"[{GetType().Name}] Starting S3 upload for file: {fileName}");

            string bucketName = _configuration["AWS:BucketName"];
            string s3Folder = _configuration["AWS:BucketFolder"];
            string objectKey = $"{s3Folder}/{Guid.NewGuid()}_{fileName}";

            try
            {
                long fileSize = fileStream.Length;

                var putRequest = new PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = objectKey,
                    InputStream = fileStream,
                    ContentType = contentType
                };

                var response = await _s3Client.PutObjectAsync(putRequest);

                _logger.LogInformation(
                    $"[{GetType().Name}] Successfully uploaded file to S3. Bucket: {bucketName}, Key: {objectKey}, Size: {fileSize} bytes");

                return (bucketName, objectKey, fileSize);
            }
            catch (AmazonS3Exception ex)
            {
                _logger.LogError($"[{GetType().Name}] S3 upload error: {ex.Message}");
                throw;
            }
        }

        public async Task<Stream> DownloadFileAsync(string bucketName, string objectKey)
        {
            _logger.LogInformation($"[{GetType().Name}] Starting S3 download. Bucket: {bucketName}, Key: {objectKey}");

            try
            {
                var response = await _s3Client.GetObjectAsync(bucketName, objectKey);
                _logger.LogInformation($"[{GetType().Name}] Successfully downloaded file from S3");
                return response.ResponseStream;
            }
            catch (AmazonS3Exception ex)
            {
                _logger.LogError($"[{GetType().Name}] S3 download error: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteFileAsync(string bucketName, string objectKey)
        {
            _logger.LogInformation($"[{GetType().Name}] Starting S3 deletion. Bucket: {bucketName}, Key: {objectKey}");

            try
            {
                await _s3Client.DeleteObjectAsync(bucketName, objectKey);
                _logger.LogInformation($"[{GetType().Name}] Successfully deleted file from S3");
            }
            catch (AmazonS3Exception ex)
            {
                _logger.LogError($"[{GetType().Name}] S3 deletion error: {ex.Message}");
                throw;
            }
        }
    }
}
