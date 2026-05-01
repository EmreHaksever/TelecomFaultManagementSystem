namespace Telecom.UI.Models;

public class CreateTicketViewModel
{
    public string CustomerNo { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Priority { get; set; } = 2; // Default Medium
}
