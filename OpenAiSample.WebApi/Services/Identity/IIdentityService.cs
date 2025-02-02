using OpenAiSample.WebApi.Models.Account;

namespace OpenAiSample.WebApi.Services.Identity
{
    /// <summary>
    /// Provides an abstraction IdentityService needed for user Authentication/Authorization process
    /// </summary>
    public interface IIdentityService
    {
        Task<RegisterResponse> RegisterUserAsync(RegisterViewModel registerModel);

        Task<LoginResponse> LoginUserAsync(LoginViewModel loginModel);
    }
}
