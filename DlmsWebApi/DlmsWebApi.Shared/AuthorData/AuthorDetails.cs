using System.ComponentModel.DataAnnotations;

namespace DlmsWebApi.Shared.AuthorData
{
    public class AuthorDetails
    {
        public int AuthorId { get; set; }
        public string AuthorIdString { get; set; }
        [Required]
        public string FirstName { get; set; }
        public string? MiddleName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string? Bio { get; set; }
        [Required]
        public DateOnly DateOfBirth { get; set; }
        public string? Status { get; set; }
        public string? User { get; set; }
    }
}
