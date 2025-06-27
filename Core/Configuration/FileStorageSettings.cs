namespace Core.Configuration
{
    public class FileStorageSettings
    {
        public string BasePath { get; set; }
        public string[] AllowedFileTypes { get; set; }
        public long MaxFileSize { get; set; }
    }
} 