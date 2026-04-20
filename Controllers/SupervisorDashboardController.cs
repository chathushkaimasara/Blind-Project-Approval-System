using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlindMatchPAS.Models;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlindMatchPAS.Controllers
{
    [Authorize(Roles = "Supervisor")]
    public class SupervisorDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public SupervisorDashboardController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(List<string> selectedAreas)
        {
            // Get distinct research areas from projects for the multiselect filter
            var availableAreas = await _context.ProjectProposals
                                      .Select(p => p.ResearchArea)
                                      .Distinct()
                                      .ToListAsync();
                                      
            ViewBag.AvailableAreas = availableAreas;
            ViewBag.SelectedAreas = selectedAreas ?? new List<string>();

            // View only pending projects
            var proposalsQuery = _context.ProjectProposals.Where(p => p.Status == "Pending");

            if (selectedAreas != null && selectedAreas.Any())
            {
                proposalsQuery = proposalsQuery.Where(p => selectedAreas.Contains(p.ResearchArea));
            }

            var proposals = await proposalsQuery.AsNoTracking().ToListAsync();

            return View(proposals);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Accept(int id, List<string> selectedAreas)
        {
            var proposal = await _context.ProjectProposals.FindAsync(id);
            if (proposal == null || proposal.Status != "Pending") return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            proposal.Status = "Matched";
            proposal.SupervisorId = user.Id;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Project proposal accepted and matched.";
            
            // Re-pass the selected areas to maintain filter state
            return RedirectToAction(nameof(Index), new { selectedAreas });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id, List<string> selectedAreas)
        {
            var proposal = await _context.ProjectProposals.FindAsync(id);
            if (proposal == null || proposal.Status != "Pending") return NotFound();

            proposal.Status = "Rejected";
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Project proposal has been rejected.";
            
            // Re-pass the selected areas to maintain filter state
            return RedirectToAction(nameof(Index), new { selectedAreas });
        }
    }
}
