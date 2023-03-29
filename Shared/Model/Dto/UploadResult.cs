namespace Cargotruck.Shared.Model.Dto
{
    public class UploadResult
    {
        public bool Uploaded { get; set; }
        public string? FileName { get; set; }
        public string? StoredFileName { get; set; }
        public string? Error { get; set; } = string.Empty;
    }
}
