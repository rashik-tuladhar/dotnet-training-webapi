namespace DlmsWebApi.Repository.Models;

public abstract class BaseEntity
{
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public string? CreatedBy { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public string? ModifiedBy { get; set; }
    public string? Status { get; set; }
}
