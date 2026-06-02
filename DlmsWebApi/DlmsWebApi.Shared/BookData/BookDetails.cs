using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace DlmsWebApi.Shared.BookData
{
    public class BookDetails
    {
        public int BookId { get; set; }
        public string BookIdString{ get; set; }
        
        [Required]
        [StringLength(500)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Author { get; set; } = string.Empty;

        //public List<SelectListItem> AuthorList { get; set; } = new List<SelectListItem>();
        //public List<SelectListItem> PublicationList { get; set; } = new List<SelectListItem>();
        //public List<SelectListItem> CategoryList { get; set; } = new List<SelectListItem>();

        [Required]
        [StringLength(500)]
        public string Publication { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Category { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Isbn { get; set; } = string.Empty;

        [Required]
        [Range(1, 50)]
        public int TotalCopies { get; set; }

        [Required]
        [Range(1, 50)]
        public int AvailableCopies { get; set; }

        [Required]
        [StringLength(500)]
        public string Edition { get; set; } = string.Empty;

        public string? User { get; set; }
        public string? Status { get; set; }

        public string? ImageUrl { get; set; }

        [DataType(DataType.Upload)]
        public IFormFile? ImageFile { get; set; }
    }
}
