using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenAiSample.WebApi.Models.OpenAi;
using OpenAiSample.WebApi.Models.Requests;
using OpenAiSample.WebApi.Models.Responses;
using System.Net.Http.Headers;
using System.Text;

namespace OpenAiSample.WebApi.Services;

public class OpenAiService : IOpenAiService
{
    private readonly HttpClient _httpClient;
    private readonly string Key = "sample test fake string api key";

    public OpenAiService(HttpClient httpClient, IOptionsMonitor<OpenAiConfig> optionsMonitor)
    {
        _httpClient = httpClient;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Key);
        _httpClient.DefaultRequestHeaders.Add("OpenAI-Beta", "assistants=v2");
    }

    public async Task<CreateVectorStoreResponse> CreateVectorStoreFileAsync(string vectorStoreId, CreateVectorStoreFileRequest request)
    {
        using var httpClient = new HttpClient();
        // Set necessary headers
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Key);
        httpClient.DefaultRequestHeaders.Add("OpenAI-Beta", "assistants=v2");

        // Serialize the request body to JSON
        var jsonContent = JsonConvert.SerializeObject(request);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        // Make the POST request
        var response = await httpClient.PostAsync($"https://api.openai.com/v1/vector_stores/{vectorStoreId}/files", content);

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<CreateVectorStoreResponse>(responseContent);
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Failed to create vector store file: {response.StatusCode} - {errorContent}");
        }
    }

    public async Task<CreateAssistantResponse> CreateAssistantsAsync(CreateAssistantRequest request)
    {
        using var httpClient = new HttpClient();
        // Set necessary headers
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Key);
        httpClient.DefaultRequestHeaders.Add("OpenAI-Beta", "assistants=v2");

        // Serialize the request body to JSON
        var jsonContent = JsonConvert.SerializeObject(request);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        // Make the POST request
        var response = await httpClient.PostAsync("https://api.openai.com/v1/assistants", content);

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<CreateAssistantResponse>(responseContent);
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Failed to create assistant: {response.StatusCode} - {errorContent}");
        }
    }

    public async Task<(string response, string threadId)> SendMessageAsync(string message, string assistantId)
    {
        var cts = new CancellationTokenSource();

        var requestData = new
        {
            assistant_id = assistantId,
            thread = new
            {
                messages = new[]
                {
                new { role = "user", content = message }
            }
            }
        };

        var jsonContent = JsonConvert.SerializeObject(requestData);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        var url = "https://api.openai.com/v1/threads/runs"; // Endpoint for creating thread and run
        var response = await _httpClient.PostAsync(url, content, cts.Token);
        await Task.Delay(15000);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Failed to create thread and run: {response.StatusCode} - {errorContent}");
        }

        var jsonResponse = JObject.Parse(await response.Content.ReadAsStringAsync());
        var threadId = jsonResponse["thread_id"].ToString();
        var runId = jsonResponse["id"].ToString();

        // Check the run status
        var status = await CheckRunStatusAsync(threadId, runId, cts.Token);
        while (status == "running")
        {
            await Task.Delay(8000); // Wait for 8 seconds before checking the status again
            status = await CheckRunStatusAsync(threadId, runId, cts.Token);
        }

        if (status == "completed")
        {
            await Task.Delay(2000);

            var finalResponse = await RetrieveMessagesAsync(threadId, cts.Token);
            return (finalResponse, threadId);
        }

        return ("We are currently unable to process your request due to technical problems.", threadId);
    }

    private async Task<string> CheckRunStatusAsync(string threadId, string runId, CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync($"https://api.openai.com/v1/threads/{threadId}/runs/{runId}", cancellationToken);
        response.EnsureSuccessStatusCode();
        var jsonResponse = JObject.Parse(await response.Content.ReadAsStringAsync());
        return jsonResponse["status"].ToString();
    }

    private async Task<string> RetrieveMessagesAsync(string threadId, CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync($"https://api.openai.com/v1/threads/{threadId}/messages", cancellationToken);
        response.EnsureSuccessStatusCode();
        var jsonResponse = JObject.Parse(await response.Content.ReadAsStringAsync());

        // Access the 'data' array which contains the messages
        var messages = jsonResponse["data"].Children().ToList();

        // Check if there are any messages
        if (!messages.Any())
        {
            return "No response received.";
        }

        // Filter for messages where the role is 'assistant'
        var assistantMessages = messages.Where(m => m["role"].ToString() == "assistant").ToList();

        // Check if there are any assistant messages
        if (!assistantMessages.Any())
        {
            return "No response from assistant.";
        }

        // Get the last assistant message from the list
        var lastAssistantMessage = assistantMessages.Last();

        // Extract the 'content' array from the last assistant message
        var contentArray = lastAssistantMessage["content"];

        // Assuming the content array is not empty and the first item is of type 'text'
        if (contentArray.Any() && contentArray.First()["type"].ToString() == "text")
        {
            // Return the 'value' of the 'text' object
            return contentArray.First()["text"]["value"].ToString();
        }

        return "No readable content found.";
    }
}
