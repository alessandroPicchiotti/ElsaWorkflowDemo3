namespace ElsaGuides.ContentApproval.Client.Services;
using System.Net.Http.Json;
using ElsaGuides.ContentApproval.Shared.Models;
public interface IDocumentService
{
    Task<ApiResponse> SubmitDocumentAsync(Document document);
    Task<ApiResponse> GetWorkflowStatusAsync(string workflowInstanceId);
}

public class DocumentService : IDocumentService
{
    private readonly HttpClient _httpClient;

    public DocumentService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ApiResponse> SubmitDocumentAsync(Document document)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/documents/submit", document);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ApiResponse>()
                    ?? new ApiResponse { Success = false, Message = "Invalid response" };
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            return new ApiResponse
            {
                Success = false,
                Message = $"Error: {response.StatusCode} - {errorContent}"
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse
            {
                Success = false,
                Message = $"Exception: {ex.Message}"
            };
        }
    }

    public async Task<ApiResponse> GetWorkflowStatusAsync(string workflowInstanceId)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<ApiResponse>(
                $"/api/documents/status/{workflowInstanceId}");

            return response ?? new ApiResponse { Success = false, Message = "Invalid response" };
        }
        catch (Exception ex)
        {
            return new ApiResponse
            {
                Success = false,
                Message = $"Exception: {ex.Message}"
            };
        }
    }
}

