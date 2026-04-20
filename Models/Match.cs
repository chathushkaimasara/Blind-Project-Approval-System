using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlindMatchPAS.Models
{
    public class Match
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProjectProposalId { get; set; }

        [ForeignKey("ProjectProposalId")]
        public virtual ProjectProposal ProjectProposal { get; set; }

        [Required]
        public string SupervisorId { get; set; }

        [ForeignKey("SupervisorId")]
        public virtual ApplicationUser Supervisor { get; set; }

        [Required]
        public string StudentId { get; set; }

        [ForeignKey("StudentId")]
        public virtual ApplicationUser Student { get; set; }

        public DateTime MatchDate { get; set; } = DateTime.UtcNow;
    }
}
