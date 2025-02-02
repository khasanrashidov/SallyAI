namespace OpenAiSample.WebApi.Services.Identity
{
    public interface ICurrentUserService
    {
        string? UserEmail { get; }

        string? UserRole { get; }
    }
}
