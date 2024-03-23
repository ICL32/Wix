using System.ComponentModel.DataAnnotations;

namespace Wix.Models
{
    public class StoreModel
    {
        [Key]
        [Required]
        [StringLength(30)]
        public required string Id { get; set; }

        [Required]
        [StringLength(100)]
        public required string Title { get; set; }

        [Required]
        [StringLength(2000)]
        public required string Content { get; set; }

        public int Views { get; set; }

        public int TimeStamp { get; set; }
    }
}
