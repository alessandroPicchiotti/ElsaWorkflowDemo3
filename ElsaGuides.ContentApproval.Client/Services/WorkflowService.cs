using ElsaGuides.ContentApproval.Shared.Models;
using System.Net.Http.Json;

namespace ElsaGuides.ContentApproval.Client.Services;

public interface IWorkflowService
{
    Task<List<WorkflowInstance>> GetWorkflowInstancesAsync();
    Task<WorkflowInstance?> GetWorkflowInstanceAsync(string id);
}

public class WorkflowService : IWorkflowService
{
    private readonly HttpClient _httpClient;

    public WorkflowService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<WorkflowInstance>> GetWorkflowInstancesAsync()
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<List<WorkflowInstance>>("/api/workflows");
            return result ?? new List<WorkflowInstance>();
        }
        catch
        {
            return new List<WorkflowInstance>();
        }
    }

    public async Task<WorkflowInstance?> GetWorkflowInstanceAsync(string id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<WorkflowInstance>($"/api/workflows/{id}");
        }
        catch
        {
            return null;
        }
    }
}
