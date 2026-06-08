using DlmsWebApi.Repository.Models;
using System.ComponentModel.DataAnnotations;

namespace DlmsWebApi.Repository.Models
{
    public class Publication : BaseEntity
    {
        [Key]
        public int PublicationId { get; set; }
        [Required]
        public string PublicationName { get; set; }
        [Required]
        public string? PublicationAddress { get; set; }
        [Required]
        public string? ContactPersonName { get; set; }
        [Required]
        public string? ContactPhone { get; set; }
        [Required]
        public string? PublicationEmail { get; set; }
        public string? PublicationWebsite { get; set; }
    }
}
