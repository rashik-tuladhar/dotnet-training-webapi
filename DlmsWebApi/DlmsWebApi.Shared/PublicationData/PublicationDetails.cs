using System.ComponentModel.DataAnnotations;

namespace DlmsWebApi.Shared.PublicationData
{
    public class PublicationDetails
    {
        public int PublicationId { get; set; }
        public string? PublicationIdString { get; set; }

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
        public string? Status { get; set; }
        public string? User { get; set; }
    }
}
