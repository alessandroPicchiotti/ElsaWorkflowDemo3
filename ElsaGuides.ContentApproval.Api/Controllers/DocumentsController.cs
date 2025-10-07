using Elsa.Workflows.Runtime;
using Elsa.Workflows.Runtime.Parameters;
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
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse>> SubmitDocument([FromBody] Document document)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Invalid document data"
                });
            }

            // Trigger workflow usando HTTP endpoint
            var input = new Dictionary<string, object>
            {
                ["Document"] = document
            };
            var inputStart = new StartWorkflowRuntimeParams()
            {
                Input = new Dictionary<string, object>
                {
                    ["Document"] = input
                }
            };
            // In Elsa 3, puoi triggerare workflow in vari modi
            // Questo è un esempio usando StartWorkflowAsync
            var result = await _workflowRuntime.StartWorkflowAsync(
                "DocumentApprovalWorkflow", // Definition ID o Name
                inputStart
            );

            _logger.LogInformation("Workflow started for document {DocumentId}", document.Id);

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Document submitted successfully for approval",
                Data = new { WorkflowInstanceId = result.WorkflowInstanceId }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting document");
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = $"Error: {ex.Message}"
            });
        }
    }

    [HttpGet("status/{workflowInstanceId}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse>> GetWorkflowStatus(string workflowInstanceId)
    {
        try
        {
            // Recupera lo stato del workflow
            // In Elsa 3, userai IWorkflowInstanceStore
            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Workflow status retrieved",
                Data = new { WorkflowInstanceId = workflowInstanceId }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting workflow status");
            return BadRequest(new ApiResponse
            {
                Success = false,
                Message = $"Error: {ex.Message}"
            });
        }
    }
}