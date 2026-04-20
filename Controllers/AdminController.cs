using BlindMatchPAS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BlindMatchPAS.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public AdminController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalUsers = await _userManager.Users.CountAsync();
            ViewBag.TotalProjects = await _context.ProjectProposals.CountAsync();
            ViewBag.TotalMatches = await _context.ProjectProposals.Where(p => p.SupervisorId != null).CountAsync();
            ViewBag.TotalResearchAreas = await _context.ResearchAreas.CountAsync();

            return View();
        }

        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users.ToListAsync();
            var userViewModels = new List<UserListViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userViewModels.Add(new UserListViewModel
                {
                    Id = user.Id,
                    Email = user.Email ?? "",
                    Role = string.Join(", ", roles)
                });
            }

            return View(userViewModels);
        }

        public IActionResult CreateUser()
        {
            ViewBag.Roles = new SelectList(_roleManager.Roles, "Name", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(model.Role))
                    {
                        await _userManager.AddToRoleAsync(user, model.Role);
                    }
                    return RedirectToAction(nameof(Users));
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            ViewBag.Roles = new SelectList(_roleManager.Roles, "Name", "Name");
            return View(model);
        }

        public async Task<IActionResult> EditUser(string id)
        {
            if (id == null) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);

            var model = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email ?? "",
                Role = roles.FirstOrDefault() ?? ""
            };

            ViewBag.Roles = new SelectList(_roleManager.Roles, "Name", "Name");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.Id);
                if (user == null) return NotFound();

                user.Email = model.Email;
                user.UserName = model.Email;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    var currentRoles = await _userManager.GetRolesAsync(user);
                    await _userManager.RemoveFromRolesAsync(user, currentRoles);

                    if (!string.IsNullOrEmpty(model.Role))
                    {
                        await _userManager.AddToRoleAsync(user, model.Role);
                    }

                    return RedirectToAction(nameof(Users));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            ViewBag.Roles = new SelectList(_roleManager.Roles, "Name", "Name");
            return View(model);
        }

        public async Task<IActionResult> ResearchAreas()
        {
            return View(await _context.ResearchAreas.ToListAsync());
        }

        public IActionResult CreateResearchArea()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateResearchArea([Bind("Name")] ResearchArea researchArea)
        {
            if (ModelState.IsValid)
            {
                _context.Add(researchArea);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ResearchAreas));
            }
            return View(researchArea);
        }

        public async Task<IActionResult> EditResearchArea(int? id)
        {
            if (id == null) return NotFound();

            var researchArea = await _context.ResearchAreas.FindAsync(id);
            if (researchArea == null) return NotFound();

            return View(researchArea);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditResearchArea(int id, [Bind("Id,Name")] ResearchArea researchArea)
        {
            if (id != researchArea.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(researchArea);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ResearchAreaExists(researchArea.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(ResearchAreas));
            }
            return View(researchArea);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteResearchArea(int id)
        {
            var researchArea = await _context.ResearchAreas.FindAsync(id);
            if (researchArea != null)
            {
                _context.ResearchAreas.Remove(researchArea);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(ResearchAreas));
        }

        private bool ResearchAreaExists(int id)
        {
            return _context.ResearchAreas.Any(e => e.Id == id);
        }

        public async Task<IActionResult> Matches()
        {
            var projects = await _context.ProjectProposals
                .Include(p => p.Student)
                .Include(p => p.Supervisor)
                .Where(p => p.SupervisorId != null)
                .ToListAsync();

            var model = projects.Select(p => new MatchListViewModel
            {
                ProjectId = p.Id,
                Title = p.Title,
                StudentEmail = p.Student?.Email ?? "Unknown",
                SupervisorEmail = p.Supervisor?.Email ?? "Unknown",
                ResearchArea = p.ResearchArea,
                Status = p.Status
            }).ToList();

            return View(model);
        }

        public async Task<IActionResult> ReassignProject(int id)
        {
            var project = await _context.ProjectProposals
                .Include(p => p.Supervisor)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null || project.SupervisorId == null) return NotFound();

            var supervisors = await _userManager.GetUsersInRoleAsync("Supervisor");

            var model = new ReassignProjectViewModel
            {
                ProjectId = project.Id,
                ProjectTitle = project.Title,
                CurrentSupervisorId = project.SupervisorId,
                CurrentSupervisorEmail = project.Supervisor?.Email ?? "Unknown"
            };

            ViewBag.Supervisors = new SelectList(supervisors, "Id", "Email");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReassignProject(ReassignProjectViewModel model)
        {
            if (ModelState.IsValid)
            {
                var project = await _context.ProjectProposals.FindAsync(model.ProjectId);
                if (project == null) return NotFound();

                project.SupervisorId = model.NewSupervisorId;
                _context.Update(project);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Matches));
            }

            var supervisors = await _userManager.GetUsersInRoleAsync("Supervisor");
            ViewBag.Supervisors = new SelectList(supervisors, "Id", "Email");
            return View(model);
        }
    }
}
