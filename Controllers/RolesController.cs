using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shops.Data;
using System;
using System.Collections.Generic;
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
        public IActionResult Create()
        {
            IdentityRole role = new IdentityRole { };
            return View(role);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IdentityRole identityRole)
        {
            if (!ModelState.IsValid)
                return View();

            if (identityRole.Name == null)
            {
                ModelState.AddModelError("Name", "Name is Empty !");
                return View(identityRole);
            }
            if (await _dbContext.Roles.AnyAsync(n => n.Name == identityRole.Name))
            {
                ModelState.AddModelError("Name", "Name is Exist !");
                return View(identityRole);
            }
            
            IdentityRole role = new IdentityRole
            {
                Name = identityRole.Name
            };
            await _roleManager.CreateAsync(role);

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Edit(string id)
        {
            var role = await _dbContext.Roles.FindAsync(id);
            if (role is null)
                return NotFound();
            return View(role);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(IdentityRole identityRole)
        {
            if (!ModelState.IsValid)
                return View();
            var role = await _dbContext.Roles.FindAsync(identityRole.Id);

            if (role is null)
                return NotFound();

            if (identityRole.Name == null)
            {
                ModelState.AddModelError("Name", "Name is Empty!");
                return View();
            }
            if (await _dbContext.Roles.AnyAsync(n => n.Name == identityRole.Name))
            {
                ModelState.AddModelError("Name", "Name is Exist !");
                return View();
            }
           
            role.Name = identityRole.Name;
            role.NormalizedName = identityRole.Name.ToUpper();
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
