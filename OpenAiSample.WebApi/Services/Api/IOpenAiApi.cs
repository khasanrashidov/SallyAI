using OpenAiSample.WebApi.Models.Requests;
using OpenAiSample.WebApi.Models.Responses;
using RestEase;

namespace OpenAiSample.WebApi.Services.Api;

public interface IOpenAiApi
{
    [Post("files")]
    Task<UploadFileResponse> UploadFileAsync([Body] HttpContent content);

    [Post("vector_stores")]
    Task<CreateVectorStoreResponse> CreateVectorStoreAsync(
        [Body] CreateVectorStoreRequest request,
        [Header("Content-Type")] string contentType = "application/json",
        [Header("OpenAI-Beta")] string openAiBeta = "assistants=v2");
}
