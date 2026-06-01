namespace DlmsWebApi.Repository.Models;

public abstract class BaseEntity
{
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public string? ModifiedBy { get; set; }
    public string? Status { get; set; }
}
