using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shops.Data;
using Shops.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shops.Controllers
{
    [Authorize(Roles = "Admin")]
    public class MarkaController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public MarkaController(ApplicationDbContext applicationDbContext)
        {
            _dbContext = applicationDbContext;
        }
        public async Task<IActionResult> Index()
        {

            return View(await _dbContext.Markas.OrderBy(m=>m.Name).ToListAsync());
        }

        public async Task<IActionResult> Modify(int? id)
        {
            if (id == null)
            {
                Marka marka = new Marka { };
                return View(marka);
            }
            var CheckMarka = await _dbContext.Markas.FindAsync(id);
            if (CheckMarka is null)
                return NotFound();
            return View(CheckMarka);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Modify(Marka Marka)
        {
            if (!ModelState.IsValid)
                return View(ModelState);

            if (Marka.Name == null)
            {
                ModelState.AddModelError("Name", "Name is Empty!");
                return View(Marka);
            }

            if (await _dbContext.Markas.AnyAsync(n => n.Name == Marka.Name))
            {
                ModelState.AddModelError("Name", "Name is Exist !");
                return View(Marka);
            }


            if (Marka.Id <= 0)
            {
                Marka marka = new Marka
                {
                    Name = Marka.Name
                };
                await _dbContext.Markas.AddAsync(marka);
            }
            else
            {
                var marka = await _dbContext.Markas.FindAsync(Marka.Id);

                if (marka is null)
                    return NotFound();

                marka.Name = Marka.Name;
            }
            await _dbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? Id)
        {
            if (Id == null)
                return BadRequest();
            var _marka = await _dbContext.Markas.FindAsync(Id);
            if (_marka == null)
                return NotFound();
            _dbContext.Markas.Remove(_marka);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
