using OpenAiSample.WebApi.Models.BaseModels;

namespace OpenAiSample.WebApi.Models.Account
{
    /// <summary>
    /// LoginResponse is used for login response
    /// </summary>
    public class LoginResponse : BaseResponse
    {
        public TokenModel Result { get; set; }
    }
}
