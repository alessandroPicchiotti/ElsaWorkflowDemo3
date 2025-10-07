using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElsaGuides.ContentApproval.Shared.Models;

public class Document
{
    [Required(ErrorMessage = "Document ID is required")]
    public string Id { get; set; } = string.Empty;

    [Required]
    public Author Author { get; set; } = new();

    [Required(ErrorMessage = "Document body is required")]
    [MinLength(10, ErrorMessage = "Document body must be at least 10 characters")]
    public string Body { get; set; } = string.Empty;
}

public class Author
{
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; } = string.Empty;
}

public class DocumentSubmissionResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? WorkflowInstanceId { get; set; }
}
