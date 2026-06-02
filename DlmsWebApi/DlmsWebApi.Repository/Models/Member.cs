using System.ComponentModel.DataAnnotations;

namespace DlmsWebApi.Repository.Models;

public class Member : BaseEntity
{
    [Key]
    public int MemberId { get; set; }

    [Required]
    public string MemberName { get; set; } = string.Empty;

    [Required]
    public long PhoneNumber { get; set; }

    [Required]
    public string Address { get; set; } = string.Empty;

    [Required]
    public string Email { get; set; } = string.Empty;

    [Required]
    public DateTime JoinedDate { get; set; }

    [Required]
    public DateTime ExpirationDate { get; set; }

    [Required]
    public string MembershipType { get; set; } = string.Empty;

    public string? Status { get; set; }
}