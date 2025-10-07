using Elsa.Workflows.Management;
using Elsa.Workflows.Management.Filters;
using Elsa.Workflows.Runtime;
using Elsa.Workflows.Runtime.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace ElsaGuides.ContentApproval.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkflowsController : ControllerBase
{
    private readonly IWorkflowInstanceStore _workflowInstanceStore;
    private readonly ILogger<WorkflowsController> _logger;

    public WorkflowsController(
        IWorkflowInstanceStore workflowInstanceStore,
        ILogger<WorkflowsController> logger)
    {
        _workflowInstanceStore = workflowInstanceStore;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult> GetWorkflowInstances(
        [FromQuery] int page = 0,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            // In Elsa 3, recupera le istanze usando IWorkflowInstanceStore
            var instances = await _workflowInstanceStore.FindManyAsync(
                new WorkflowInstanceFilter
                {
                    // Aggiungi filtri se necessario
                },
                cancellationToken: HttpContext.RequestAborted
            );

            var result = instances.Select(i => new
            {
                Id = i.Id,
                WorkflowDefinitionId = i.DefinitionId,
                WorkflowName = i.DefinitionVersionId, // Adatta secondo la tua necessità
                Status = i.Status.ToString(),
                CreatedAt = i.CreatedAt,
                FinishedAt = i.FinishedAt
            });

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving workflow instances");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetWorkflowInstance(string id)
    {
        try
        {
            var instanceFilter= new WorkflowInstanceFilter
            {
                Id = id
            };
            var instance = await _workflowInstanceStore.FindAsync(
                instanceFilter,
                HttpContext.RequestAborted
            );

            if (instance == null)
                return NotFound();

            return Ok(new
            {
                Id = instance.Id,
                WorkflowDefinitionId = instance.DefinitionId,
                Status = instance.Status.ToString(),
                CreatedAt = instance.CreatedAt,
                FinishedAt = instance.FinishedAt,
                // Variables = instance.Variables // Se necessario
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving workflow instance {Id}", id);
            return StatusCode(500, new { error = ex.Message });
        }
    }
}