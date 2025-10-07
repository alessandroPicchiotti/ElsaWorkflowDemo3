using Elsa.Workflows.Runtime;
using ElsaGuides.ContentApproval.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace ElsaGuides.ContentApproval.Api.Controllers;


[ApiController]
[Route("api/[controller]")]
public class DocumentsController : ControllerBase
{
    private readonly IWorkflowRuntime _workflowRuntime;
    private readonly ILogger<DocumentsController> _logger;

    public DocumentsController(
        IWorkflowRuntime workflowRuntime,
        ILogger<DocumentsController> logger)
    {
        _workflowRuntime = workflowRuntime;
        _logger = logger;
    }

    [HttpPost("submit")]
    public async Task<ActionResult<DocumentSubmissionResponse>> SubmitDocument(
        [FromBody] Document document)
    {
        try
        {
            _logger.LogInformation("Submitting document {DocumentId} from {AuthorName}",
                document.Id, document.Author.Name);

            // Trigger workflow tramite HTTP endpoint
            // Il workflow sarà triggerato dall'HTTP Endpoint attività configurata nel designer
            var input = new Dictionary<string, object>
            {
                ["Document"] = document
            };

            // Opzionale: se vuoi triggare il workflow programmaticamente invece che via HTTP
            // var workflowInstance = await _workflowRuntime.StartWorkflowAsync(
            //     "DocumentApprovalWorkflow", 
            //     input);

            return Ok(new DocumentSubmissionResponse
            {
                Success = true,
                Message = "Document submitted successfully! The reviewer will be notified.",
                // WorkflowInstanceId = workflowInstance.Id
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting document");
            return StatusCode(500, new DocumentSubmissionResponse
            {
                Success = false,
                Message = $"Error: {ex.Message}"
            });
        }
    }

    [HttpGet("status/{workflowInstanceId}")]
    public async Task<ActionResult> GetWorkflowStatus(string workflowInstanceId)
    {
        // Implementa logica per recuperare lo stato del workflow
        return Ok(new { status = "pending" });
    }
}

