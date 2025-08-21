public class VendorApprovalDto
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public required string Status { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
}
