using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shops.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Shops.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _dbContext;

        public RolesController(RoleManager<IdentityRole> roleManager, ApplicationDbContext applicationDbContext)
        {
            _roleManager = roleManager;
            _dbContext = applicationDbContext;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _dbContext.Roles.OrderBy(o => o.Name).ToListAsync());
        }

        public async Task<IActionResult> Modify(string? id)
        {
            if (id == null)
            {
                IdentityRole role = new IdentityRole { Id =null};
                return View(role);
            }
            var CheckRole = await _dbContext.Roles.FindAsync(id);
            if (CheckRole is null)
                return NotFound();
            return View(CheckRole);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Modify(IdentityRole Role)
        {
            if (!ModelState.IsValid)
                return View(ModelState);

            if (Role.Name == null)
            {
                ModelState.AddModelError("Name", "Name is Empty!");
                return View(Role);
            }

            if (await _dbContext.Roles.AnyAsync(n => n.Name == Role.Name))
            {
                ModelState.AddModelError("Name", "Name is Exist !");
                return View(Role);
            }

            if ( !await _dbContext.Roles.AnyAsync(r => r.Id == Role.Id))
            {
                IdentityRole role = new IdentityRole
                {
                    Name = Role.Name,
                    NormalizedName = Role.Name.ToUpper()
                };
                await _dbContext.Roles.AddAsync(role);
            }
            else
            {
                var role = await _dbContext.Roles.FindAsync(Role.Id);

                if (role is null)
                    return NotFound();

                role.Name = Role.Name;
                role.NormalizedName = Role.Name.ToUpper();
            }
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        } 


     
        [HttpGet]
        public async Task<IActionResult> Delete(string? Id)
        {
            if (Id == null)
                return BadRequest();
            var role = await _dbContext.Roles.FindAsync(Id);
            if (role == null)
                return NotFound();
             _dbContext.Roles.Remove(role);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
