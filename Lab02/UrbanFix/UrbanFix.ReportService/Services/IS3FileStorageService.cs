namespace UrbanFix.ReportService.Services
{
    public interface IS3FileStorageService
    {
        Task<(string BucketName, string ObjectKey, long FileSize)> UploadFileAsync(
            Stream fileStream,
            string fileName,
            string contentType);

        Task<Stream> DownloadFileAsync(string bucketName, string objectKey);

        Task DeleteFileAsync(string bucketName, string objectKey);
    }
}
