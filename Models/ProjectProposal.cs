using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlindMatchPAS.Models
{
    public class ProjectProposal
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        public string Abstract { get; set; }

        [Required]
        public string TechStack { get; set; }

        [Required]
        public string ResearchArea { get; set; }

        public string Status { get; set; } = "Pending";

        [Required]
        public string StudentId { get; set; }

        [ForeignKey("StudentId")]
        public virtual IdentityUser Student { get; set; }

        public string SupervisorId { get; set; }

        [ForeignKey("SupervisorId")]
        public virtual IdentityUser Supervisor { get; set; }
    }
}
