using OpenAiSample.WebApi.Models.Requests;
using OpenAiSample.WebApi.Models.Responses;

namespace OpenAiSample.WebApi.Services;

public interface IOpenAiService
{
    Task<CreateVectorStoreResponse> CreateVectorStoreFileAsync(string vectorStoreId, CreateVectorStoreFileRequest request);

    Task<CreateAssistantResponse> CreateAssistantsAsync(CreateAssistantRequest request);

    Task<(string response, string threadId)> SendMessageAsync(string message, string assistantId);
}
