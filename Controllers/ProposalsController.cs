using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlindMatchPAS.Models;
using System.Security.Claims;

namespace BlindMatchPAS.Controllers
{
    [Authorize(Roles = "Student")]
    public class ProposalsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProposalsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var proposals = await _context.ProjectProposals
                .Where(p => p.StudentId == userId)
                .ToListAsync();
            return View(proposals);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var proposal = await _context.ProjectProposals
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (proposal == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (proposal.StudentId != userId) return Forbid();

            return View(proposal);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Abstract,TechStack,ResearchArea")] ProjectProposal proposal)
        {
            var userId = _userManager.GetUserId(User);
            proposal.StudentId = userId;
            proposal.Status = "Pending";

            if (ModelState.IsValid)
            {
                _context.Add(proposal);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "StudentDashboard");
            }
            return View(proposal);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var proposal = await _context.ProjectProposals.FindAsync(id);
            if (proposal == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (proposal.StudentId != userId) return Forbid();

            if (proposal.Status == "Matched")
            {
                TempData["ErrorMessage"] = "You cannot edit a proposal that has already been matched.";
                return RedirectToAction("Index", "StudentDashboard");
            }

            return View(proposal);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Abstract,TechStack,ResearchArea")] ProjectProposal proposal)
        {
            if (id != proposal.Id) return NotFound();

            var original = await _context.ProjectProposals.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
            if (original == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (original.StudentId != userId) return Forbid();

            if (original.Status == "Matched")
            {
                TempData["ErrorMessage"] = "You cannot edit a proposal that has already been matched.";
                return RedirectToAction("Index", "StudentDashboard");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    proposal.StudentId = userId;
                    proposal.Status = "Pending"; 
                    _context.Update(proposal);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProposalExists(proposal.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction("Index", "StudentDashboard");
            }
            return View(proposal);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var proposal = await _context.ProjectProposals
                .FirstOrDefaultAsync(m => m.Id == id);
            if (proposal == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (proposal.StudentId != userId) return Forbid();

            if (proposal.Status == "Matched")
            {
                TempData["ErrorMessage"] = "You cannot delete a proposal that has already been matched.";
                return RedirectToAction("Index", "StudentDashboard");
            }

            return View(proposal);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var proposal = await _context.ProjectProposals.FindAsync(id);
            if (proposal == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            if (proposal.StudentId != userId) return Forbid();

            if (proposal.Status == "Matched")
            {
                TempData["ErrorMessage"] = "You cannot delete a proposal that has already been matched.";
                return RedirectToAction("Index", "StudentDashboard");
            }

            _context.ProjectProposals.Remove(proposal);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "StudentDashboard");
        }

        private bool ProposalExists(int id)
        {
            return _context.ProjectProposals.Any(e => e.Id == id);
        }
    }
}
