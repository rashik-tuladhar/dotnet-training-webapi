using System.ComponentModel.DataAnnotations;

namespace DlmsWebApi.Repository.Models
{
    public class Book : BaseEntity
    {
        [Key]
        public int BookId { get; set; }
        [Required]
        [StringLength(500)]
        public string Name { get; set; }
        [Required]
        [StringLength(500)]
        public string Author { get; set; }
        [Required]
        [StringLength(500)]
        public string Publication { get; set; }
        [Required]
        [StringLength(500)]
        public string Category { get; set; }
        [Required]
        [StringLength(200)]
        public string Isbn { get; set; }
        [Required]
        [Range(1,50)]
        public int TotalCopies { get; set; }
        [Required]
        [Range(1, 50)]
        public int AvailableCopies { get; set; }
        [Required]
        [StringLength(500)]
        public string Edition { get; set; }

        [StringLength(1000)]
        public string? ImageUrl { get; set; }
    }
}
