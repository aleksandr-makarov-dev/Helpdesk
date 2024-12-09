namespace Helpdesk.API.Configuration
{
    public class ApplicationOptions
    {
        public int JwtExpiresInMinutes { get; set; }
        public string ConnectionString { get; set; } = string.Empty;
        public string MinioAccessKey { get; set; } = string.Empty;
        public string MinioSecretKey { get; set; } = string.Empty;
        public string JwtSecretKey { get; set; } = string.Empty;
        public string MinioBucketName { get; set; } = string.Empty;
        public string MinioEndpoint { get; set; } = string.Empty;
    }
}
