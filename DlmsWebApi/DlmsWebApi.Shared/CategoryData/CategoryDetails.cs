using System.ComponentModel.DataAnnotations;

namespace DlmsWebApi.Shared.CategoryData
{
    public class CategoryDetails
    {
        public int CategoryId { get; set; }
        public string CategoryIdString { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        public string? Status { get; set; }
        public string? User { get; set; }

    }
}
