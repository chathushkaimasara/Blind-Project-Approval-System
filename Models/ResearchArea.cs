using System.ComponentModel.DataAnnotations;

namespace BlindMatchPAS.Models
{
    public class ResearchArea
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
    }
}
