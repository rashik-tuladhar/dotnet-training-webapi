using System.ComponentModel.DataAnnotations;

namespace DlmsWebApi.Repository.Models
{
    public class Author : BaseEntity
    {
        [Key]
        public int AuthorId { get; set; }
        [Required]
        public string FirstName { get; set; }
        public string? MiddleName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string? Bio { get; set; }
        [Required]
        public DateOnly DateOfBirth { get; set; }
    }
}
