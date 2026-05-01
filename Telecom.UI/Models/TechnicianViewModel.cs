using System;

namespace Telecom.UI.Models;

public class TechnicianViewModel
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
