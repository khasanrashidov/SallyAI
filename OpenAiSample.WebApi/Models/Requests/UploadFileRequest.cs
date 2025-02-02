namespace OpenAiSample.WebApi.Models.Requests;

public class UploadFileRequest
{
    public IFormFile File { get; set; }

    public string Purpose { get; set; } = string.Empty;
}
