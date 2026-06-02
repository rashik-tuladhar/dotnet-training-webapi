using System.ComponentModel.DataAnnotations;

namespace DlmsWebApi.Repository.Models
{
    public class Category : BaseEntity
    {
        [Key]
        public int CategoryId { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
    }

}
