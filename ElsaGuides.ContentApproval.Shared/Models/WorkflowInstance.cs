using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElsaGuides.ContentApproval.Shared.Models;
public class WorkflowInstance
{
    public string Id { get; set; } = string.Empty;
    public string WorkflowDefinitionId { get; set; } = string.Empty;
    public string WorkflowName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
    public Dictionary<string, object>? Variables { get; set; }
}